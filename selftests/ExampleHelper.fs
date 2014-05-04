﻿module FSpec.SelfTests.ExampleHelper
open FSpec.Core
open Matchers
open DslHelper

let anExampleGroupNamed = ExampleGroup.create
let anExampleGroup = anExampleGroupNamed "dummy"

let withExamples examples exampleGroup =
    let folder grp ex = ExampleGroup.addExample ex grp
    examples |> List.fold folder exampleGroup

let withTearDownCode = ExampleGroup.addTearDown

let withChildGroup = ExampleGroup.addChildContext
let anExampleNamed name = Example.create name pass
let anExample = Example.create "dummy"
let aPassingExample = anExample (fun _ -> ())
let aFailingExample = anExample fail
let aPendingExample = anExample Dsl.pending

let createAnExampleWithMetaData metaData f =
    let metaData' = MetaData.create [metaData]
    anExample f |> Example.addMetaData metaData'

let run exampleGroup = 
    Runner.run exampleGroup (Report.create())

let runSingleExample example =
    anExampleGroup |> withExamples [example] |> run

let withSetupCode f = ExampleGroup.addSetup f
let withAnExampleWithMetaData metaData =
    createAnExampleWithMetaData metaData (fun _ -> ())
    |> ExampleGroup.addExample
let withAnExample = aPassingExample |> ExampleGroup.addExample
let withAnExampleNamed name = anExampleNamed name |> ExampleGroup.addExample

let shouldPass group =
    let report' = Runner.run group (Report.create())
    report' |> Report.success |> should equal true

let withMetaData data = MetaData.create [data] |> ExampleGroup.addMetaData