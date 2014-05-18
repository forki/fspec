﻿module FSpec.SelfTests.ExampleHelper
open FSpec.Core
open Matchers
open Dsl

let pass = fun _ -> ()
let fail = fun _ -> raise (AssertionError { Message = "failed" })
let anExampleGroupNamed = ExampleGroup.create
let anExampleGroup = anExampleGroupNamed "dummy"

let withExamples examples exampleGroup =
    let folder grp ex = ExampleGroup.addExample ex grp
    examples |> List.fold folder exampleGroup

let withMetaData data = TestDataMap.create [data] |> ExampleGroup.addMetaData
let withSetupCode = ExampleGroup.addSetup
let withTearDownCode = ExampleGroup.addTearDown

let applyNestedContext f grp = grp |> f |> ExampleGroup.addChildGroup
let withNestedGroupNamed name f = anExampleGroupNamed name |> applyNestedContext f
let withNestedGroup f = anExampleGroup |> applyNestedContext f
let anExampleNamed name = Example.create name pass
let anExample = Example.create "dummy"
let aPassingExample = anExample pass
let aFailingExample = anExample fail
let aPendingExample = anExample pending

let createAnExampleWithMetaData metaData f =
    let metaData' = TestDataMap.create [metaData]
    anExample f |> Example.addMetaData metaData'

let run exampleGroup = 
    let reporter = Helpers.TestReporter.instance
    Runner.doRun exampleGroup reporter (reporter.BeginTestRun())

let runSingleExample example =
    anExampleGroup |> withExamples [example] |> run

let withAnExampleWithMetaData metaData =
    createAnExampleWithMetaData metaData (fun _ -> ())
    |> ExampleGroup.addExample

let withExampleMetaData md = TestDataMap.create [md] |> Example.addMetaData
let withExampleCode f = Example.create "dummy" f |> ExampleGroup.addExample
let withAnExample = aPassingExample |> ExampleGroup.addExample
let withAnExampleNamed name = anExampleNamed name |> ExampleGroup.addExample

let anExampleWithMetaData data = aPassingExample |> withExampleMetaData data
