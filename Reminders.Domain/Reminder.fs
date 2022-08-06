module Reminders.Domain

open System

type NthDayOfWeek =
    { Place: int
      Day: DayOfWeek }

type MonthRule =
    | DateOfMonth of int
    | NthDayOfWeek of NthDayOfWeek

type Interval =
    | Day of int
    | Week of DayOfWeek list
    | Month of MonthRule
    | Year of int

type Recurrance =
    { Interval: Interval
      EndsOn: DateTime option }

type Reminder =
    { Id: Guid
      Text: string
      SetFor: DateTime
      Recurrance: Recurrance option }