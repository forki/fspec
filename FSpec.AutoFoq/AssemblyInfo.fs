﻿namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSpec.AutoFoq")>]
[<assembly: AssemblyVersionAttribute("0.2.1")>]
[<assembly: AssemblyFileVersionAttribute("0.2.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.2.1"
