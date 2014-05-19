module FSpec.SelfTests.MatcherV3Specs
open FSpec.Core
open Dsl
open MatchersV3
open Helpers

let specs = [
    describe "System.Object extesions" [
        describe ".Should" [
            it "works correctly on objects of type 'obj'" (fun _ ->
                (1).Should (be.equalTo 1)
            )
        ]

        describe ".ShouldNot" [
            it "works correctly on objects of type 'obj'" (fun _ ->
                let test = fun _ -> (1).ShouldNot (be.equalTo 1)
                test |> shouldFail
            )
        ]
    ]

    describe "equalTo matcher" [
        describe "should be.EqualTo" [
            it "succeeds when equal" <| fun _ ->
                let test () = 5 |> should (be.equalTo 5)
                test |> shouldPass

            it "fails when not equal" <| fun _ ->
                let test () = 5 |> should (be.equalTo 6)
                test |> getErrorMsg 
                |> should (be.equalTo "Expected 5 to be equal to 6")
        ]

        describe "shouldNot be.equalTo" [
            it "succeeds when not equal" <| fun _ ->
                5 |> shouldNot (be.equalTo 6)

            it "fails when equal" <| fun _ ->
                let test () = 5 |> shouldNot (be.equalTo 5)
                test |> getErrorMsg
                |> should (be.equalTo "Expected 5 to not be equal to 5")
        ]
    ]

    describe "True/False matchers" [
        describe "should be.True" [
            it "succeeds for true values" <| fun _ ->
                let test () = true |> should be.True
                test |> shouldPass

            it "fail for false values" <| fun _ ->
                let test () = false |> should be.True
                test |> shouldFail
        ]
        describe "should be.False" [
            it "fail for true values" <| fun _ ->
                let test () = true |> should be.False
                test |> shouldFail

            it "succeed for false values" <| fun _ ->
                let test () = false |> should be.False
                test |> shouldPass
        ]
    ]
    
    describe "Collection matchers" [
        describe "should have.length" [
            it "succeeds when length is expected" <| fun _ ->
                let test () = [1;2;3] |> should (have.length (be.equalTo 3))
                test |> shouldPass
        ]

        describe "should have.exactly" [
            it "succeeds when correct no of elements match" <| fun _ ->
                let test () = [1;2;2;3] |> should (have.exactly 2 (be.equalTo 2))
                test |> shouldPass
        ]

        describe "should have.atLeastOneElement" [
            it "succeeds when collection has one element" <| fun _ ->
                let test () = [1;2;3] |> should (have.atLeastOneElement (be.equalTo 3))
                test |> shouldPass

            it "succeeds when collection have multiple matching elements" <| fun _ ->
                let test () = [1;2;2;3] |> should (have.atLeastOneElement (be.equalTo 2))
                test |> shouldPass

            it "fails when collection has no matching element" <| fun _ ->
                let test () = [1;2;3] |> should (have.atLeastOneElement (be.equalTo 4))
                let expected =  "Expected [1; 2; 3] to contain at least one element to be equal to 4"
                test |> getErrorMsg |> should (be.equalTo expected)
        ]

        describe "shouldNot have.atLeastOneElement" [
            it "fails when collection has matching element" <| fun _ ->
                let test () = [1;2;3] |> shouldNot (have.atLeastOneElement (be.equalTo 2))
                test |> getErrorMsg
                |> should (be.equalTo "Expected [1; 2; 3] to contain no elements to be equal to 2")
        ]
    ]

    describe "exception matchers" [
        describe "failWithMsg" [
            it "succeeds when exception thrown with specified message" <| fun _ ->
                let f () = raise (new System.Exception("custom msg"))
                let test () = f |> should (throwException.withMessageContaining "custom msg")
                test |> shouldPass

            it "fails when no exception is thrown" <| fun _ ->
                let f () = ()
                let test () = f |> should (throwException.withMessageContaining "dummy")
                test |> shouldFail

            it "fails when exception thrown with different message" <| fun _ ->
                let f () = raise (new System.Exception("custom msg"))
                let test () = f |> should (throwException.withMessageContaining "wrong msg")
                test |> shouldFail
        ]
    ]
]
