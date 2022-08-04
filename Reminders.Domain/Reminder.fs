module Reminders.Domain

open System

type Day =
    | Sunday
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday

type NthDayOfWeek =
    { Place: int
      Day: Day }

type MonthRule =
    | DateOfMonth of int
    | NthDayOfWeek of NthDayOfWeek

type Interval =
    | Day of int
    | Week of Day list
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