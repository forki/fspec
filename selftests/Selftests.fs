module FSpec.SelfTests.SuiteBuilderSpecs
open FSpec.Core
open Dsl
open Matchers
open DslHelper

let specs =
    describe "Reporting" <| fun() ->
        let sut = DslHelper()

        describe "summary" <| fun () ->
            it "reports test success" <| fun _ ->
                sut.it "Is a success" pass

                let report = sut.run()
                report.summary() |> should equal  "1 run, 0 failed"

            it "reports test failures" <| fun _ ->
                sut.it "Is a failure" fail

                let report = sut.run()
                report.summary() |> should equal "1 run, 1 failed"

            it "reports pending tests" <| fun _ ->
                sut.it "Is pending" pending
                let report = sut.run()
                report.summary() |> should equal "1 run, 0 failed, 1 pending"

        describe "Running status" <| fun () ->
            it "Is reported while running" <| fun _ ->
                sut.describe "Some context" <| fun () ->
                    sut.it "has some behavior" pass
                let report = sut.run()
                report.testOutput() |> should equal ["Some context has some behavior - passed"]

            it "Reports multiple test results" <| fun _ ->
                sut.describe "Some context" <| fun() ->
                    sut.it "has some behavior" pass
                    sut.it "has some other behavior" pass

                let report = sut.run()
                let actual = report.testOutput()
                let expected = ["Some context has some behavior - passed";
                                "Some context has some other behavior - passed"]
                actual.should equal expected

            it "Reports nested contexts correctly" <| fun _ ->
                sut.describe "Some context" <| fun() ->
                    sut.describe "in some special state" <| fun() ->
                        sut.it "has some special behavior" pass

                let report = sut.run()
                let actual = report.testOutput() |> List.head
                actual |> should matchRegex "Some context in some special state has some special behavior"

        describe "Failed tests" <| fun() ->
            it "handles test failures in setup code" (fun _ ->
                sut.before (fun () -> failwith "error")
                sut.it "works" pass
                let result = sut.run()
                result.success() |> should equal false
            )
            
            it "handles test failures in teardown code" (fun _ ->
                sut.after (fun () -> failwith "error")
                sut.it "works" pass
                let result = sut.run()
                result.success() |> should equal false
            )

            it "Writes the output to the test report" <| fun _ ->
                sut.it "Is a failing test" <| fun _ ->
                    (5).should equal 6
                let result = sut.run()
                let actual = result.failedTests() |> List.reduce (+)
                actual |> should matchRegex "expected 5 to equal 6"

            it "write the right output for comparison tests" <| fun _ ->
                sut.it "Is a failing test" <| fun _ ->
                    5 |> should be.greaterThan 6
                let result = sut.run()
                let actual = result.failedTests() |> List.reduce (+)
                actual |> should matchRegex "expected 5 to be greater than 6"

            it "Is empty when no tests fail" <| fun _ ->
                sut.it "Is a passing test" pass
                let result = sut.run()
                let actual = result.failedTests() |> List.length
                actual.should equal 0

        describe "Tests with errors" <| fun() ->
            it "writes the exception name" <| fun _ ->
                sut.it "Is a failing test" <| fun _ ->
                    raise (new System.NotImplementedException())
                    ()
                let result = sut.run()
                let actual = result.failedTests() |> List.reduce (+)
                actual |> should matchRegex "NotImplementedException"
