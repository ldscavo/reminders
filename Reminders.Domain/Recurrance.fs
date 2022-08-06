module Recurrance.Domain.Recurrance

open System
open Reminders.Domain

let rec private nextDayOfWeek days (date: DateTime) =
    let newDate = date.AddDays 1

    match (days |> List.contains newDate.DayOfWeek) with
    | true -> newDate
    | false -> nextDayOfWeek days newDate

let rec private nthDayOfWeek i dayOfWk (date: DateTime) =
    let newDate = date.AddDays 1
    let count = if newDate.DayOfWeek = dayOfWk.Day then i + 1 else i
    
    match (count = dayOfWk.Place) with
    | true -> newDate
    | false -> nthDayOfWeek count dayOfWk newDate

let private nextMonth (monthRule: MonthRule) (date: DateTime) =
    match monthRule with
    | DateOfMonth day -> date.AddMonths 1
    | NthDayOfWeek dayOfWk -> nthDayOfWeek 0 dayOfWk date

let nextRecurrance (date: DateTime) (recurrance: Recurrance) =
    match recurrance.Interval with
    | Day cnt -> date.AddDays cnt
    | Week days -> nextDayOfWeek days date
    | Month rule -> nextMonth rule date
    | Year cnt -> date.AddYears cnt

let nextReminder (reminder: Reminder) =
    match reminder.Recurrance with
    | Some recurrance ->
        { reminder with
            SetFor = (nextRecurrance reminder.SetFor recurrance) }
    | None -> reminder