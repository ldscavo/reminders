module Reminders.Domain.Tests.Recurrance

open System
open NUnit.Framework
open FsUnit
open FsCheck.NUnit
open Reminders.Domain
open Recurrance.Domain.Recurrance
open FsCheck

let newReminder () =
    { Id = Guid.NewGuid ()
      Text = "Eat the lawn"
      SetFor = DateTime(2022, 09, 05)
      Recurrance = None }

let mutable reminder = newReminder()

type TestDay = int

let genDay : Gen<TestDay> =
    gen {
        return! Gen.choose (1, 28)
    }

type GenTestDay =
    static member Day = Arb.fromGen genDay

[<SetUp>]
let setUp () =
    Arb.register<GenTestDay> () |> ignore
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

[<Property>]
let ``weekly interval falls on the next specified day of the week`` (dayOfWeek: DayOfWeek) =
    let weeklyReminder = 
        { reminder with
            Recurrance = Some { Interval = Week [dayOfWeek]; EndsOn = None } }

    let newReminder = weeklyReminder |> nextReminder

    newReminder.Recurrance |> should equal weeklyReminder.Recurrance
    newReminder.SetFor.DayOfWeek |> should equal dayOfWeek

[<Property>]
let ``weekly interval is always set within a week of the specified date`` (dayOfWeek: DayOfWeek) =
    let weeklyReminder = 
        { reminder with
            Recurrance = Some { Interval = Week [dayOfWeek]; EndsOn = None } }

    let newReminder = weeklyReminder |> nextReminder

    let difference = newReminder.SetFor.Subtract reminder.SetFor
    difference.Duration().Days |> should (be lessThanOrEqualTo) 7

[<Test>]
let ``a weekly interval with multiple days of the week must set on each day`` () =
    let reminderDays = [DayOfWeek.Monday; DayOfWeek.Wednesday; DayOfWeek.Friday]

    let weeklyReminder = 
        { reminder with
            SetFor = DateTime(2022, 09, 04) // Sunday
            Recurrance = Some { Interval = Week reminderDays; EndsOn = None } }
    
    let firstReminder = weeklyReminder |> nextReminder
    firstReminder.SetFor.DayOfWeek |> should equal reminderDays[0]

    let secondReminder = firstReminder |> nextReminder
    secondReminder.SetFor.DayOfWeek |> should equal reminderDays[1]

    let thirdReminder = secondReminder |> nextReminder
    thirdReminder.SetFor.DayOfWeek |> should equal reminderDays[2]

[<Test>]
let ``monthly recurrance always falls on the same day when possible`` () =      
    let test day =
        let monthlyReminder = 
            { reminder with
                SetFor = DateTime(2022, 09, day)
                Recurrance = Some { Interval = Month (DateOfMonth day); EndsOn = None } }
        let nextReminder = monthlyReminder |> nextReminder
        
        nextReminder.SetFor.Month |> should equal (monthlyReminder.SetFor.Month + 1)
        nextReminder.SetFor.Day |> should equal day
    
    [1..28] |> Seq.iter test