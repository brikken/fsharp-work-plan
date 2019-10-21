module Model

[<AutoOpen; RequireQualifiedAccess>]
module Day =
    type Day = private Day of y: int * m: int * d: int
    type private Create = int * int * int -> Day
    type private Value = Day -> int * int * int
[<AutoOpen; RequireQualifiedAccess>]
module Month =
    type Month = private Month of y: int * m: int
    type private Create = int * int -> Month
    type private Value = Month -> int * int
type JobId = JobId of int
type DayConvention = WorkDays | Weekends
type JobSchedule = Daily of DayConvention | Monthly
type JobDue = DueDay of DayConvention * int | DuePeriod of DayConvention * int * int
type DueDay = DueWorkDay of Day | DueWeekendDay of Day
type JobDefinition = {
    jobId: JobId
    jobName: string
    schedule: JobSchedule
    due: JobDue
}
type AsOfDay = AsOfWorkDay of Day | AsOfWeekendDay of Day
type AsOf = AsOfDay of AsOfDay | AsOfMonth of Month
type PersonId = PersonId of int
type GroupId = GroupId of int
type Assignee = Person of PersonId | Group of GroupId
type Assignment = {
    jobId: JobId
    asOf: AsOf
    assignee: Assignee
}
type PropertyFilter<'T> = Any | Only of 'T list
type Filter = { jobIds: PropertyFilter<JobId>; asOfs: PropertyFilter<AsOf>; assignees: PropertyFilter<Assignee>; dueDays: PropertyFilter<Day>; }
type Dimension = JobDimension | PersonDimension | AsOfDayDimension | AsOfMonthDimension | DueDayDimension // Type of dimension manages partitioning (incl. filtering of (potential) assignments)
type DimensionPartition<'T> = { item: 'T; filter: Filter; }
type DimensionPartitioned =
    | JobPartitioned of DimensionPartition<JobId> list
    | DayPartitioned of DimensionPartition<Day> list
    | MonthPartitioned of DimensionPartition<Month> list
    | AssigneePartitioned of DimensionPartition<Assignee> list
type PartitionAssignments = DimensionPartitioned -> Assignment list -> Assignment list list
module Table =
    module Cell =
        module State =
            type AddEditValue = AddEditValue of string
            type Addable = Addable
            type Editable = Editable of Assignment
            type Saveable = Add of AddEditValue | Edit of Assignment * AddEditValue
        module Navigation = // TODO: Finish navigation
            type Direction = Previous | Next
        type Navigate = Navigate of Navigation.Direction list // TODO: Should only be created by the Table aggregate
        type State =
            | AddSelect of State.Addable
            | AssignmentSelect of State.Editable
            | Save of State.Saveable
        type Id = Id of int list // TODO: Should only be created by the Table aggregate
        type Header = Header of string
        type Assignment = {
            id: Id
            assignments: Assignment list
            navigations: Navigate list
        }
    type Cell =
        | Header of Cell.Header
        | Assignment of Cell.Assignment
    module State =
        type Editable = Editable of Cell.Id * Cell.State.Editable
    type State =
        | None 
        | Cell of Cell.Id
        | CellItem of Cell.Id * Cell.State
    type Contents =
        | ZeroDim of Cell
        | OneDim of Cell list
        | TwoDim of Cell list list
        | ThreeDim of Cell list list list
type Table = {
    contents: Table.Contents
    selection: Table.State
}
module Model =
    type Config = { dimensions: Dimension list; }
    type Data = {
        assignments: Assignment list
        jobDefinitions: JobDefinition list
        assignees: Assignee list
        asOfs: AsOf list
    }
type Model = {
    modelConfig: Model.Config
    modelData: Model.Data
}
module AddEdit =
    module Command =
        type Add = Add
        type Edit = Edit
        type Save = Save of Table.Cell.State.AddEditValue
        type Cancel = Cancel
        type GetAdd = Table.Cell.State.Addable -> Add
        type GetEdit = Table.Cell.State.Editable -> Edit
        type GetSave = Table.Cell.State.Saveable -> Save
        type GetCancel = Table.Cell.State.Saveable -> Cancel
    type Command =
        | Edit of Command.Edit
        | SaveEdit of Command.Save
        | CancelEdit of Command.Cancel
        | Add of Command.Add
        | SaveAdd of Command.Save
        | CancelAdd of Command.Cancel
type Command =
    | SelectCell of Table.Cell.Id
    | EditCell
    | SelectAssignment of Assignment
    | AddEdit of AddEdit.Command
type ScheduleAsOfFit = JobSchedule -> AsOf -> bool
type RenderDay = Day -> string
type RenderJobDefinition = JobDefinition -> string
type RenderAssignee = Assignee -> string
type RenderHeaders = DimensionPartitioned -> string list
type GetTable = Model -> Table
type Update = Command -> Model -> Model
type ExpandDimension = Model.Data -> Dimension -> DimensionPartitioned
type PropertyFilterIntersection<'T> = PropertyFilter<'T> -> PropertyFilter<'T> -> PropertyFilter<'T>
type FilterIntersection = Filter -> Filter -> Filter
