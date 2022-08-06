module Reminders.Domain.Tests.Recurrance

open System
open NUnit.Framework
open FsUnit
open FsCheck.NUnit
open Reminders.Domain
open Recurrance.Domain.Recurrance

let newReminder () =
    { Id = Guid.NewGuid ()
      Text = "Eat the lawn"
      SetFor = DateTime(2022, 09, 05)
      Recurrance = None }

let mutable reminder = newReminder()

[<SetUp>]
let setUp () =
    reminder <- newReminder ()

[<Test>]
let ``reminder without recurrance returns itself`` () =
    let newReminder = reminder |> nextReminder

    newReminder |> should equal reminder

[<Property>]
let ``daily interval returns the same time the next day`` (n: int) =
    let dailyReminder =
        { reminder with
            Recurrance = Some { Interval = Day n; EndsOn = None } }
    
    let newReminder = dailyReminder |> nextReminder

    newReminder.Recurrance |> should equal dailyReminder.Recurrance

    let nDaysLater = (dailyReminder.SetFor.AddDays n)
    newReminder.SetFor |> should equal nDaysLater