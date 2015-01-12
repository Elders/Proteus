#I @"./bin/tools/FAKE/tools/"
#r @"./bin/tools/FAKE/tools/FakeLib.dll"
#load @"./bin/tools/SourceLink.Fake/tools/SourceLink.fsx"

open Fake
open Fake.Git
open Fake.FSharpFormatting
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System
open System.IO

type System.String with member x.contains (comp:System.StringComparison) str = x.IndexOf(str,comp) >= 0        
let excludePaths (pathsToExclude : string list) (path: string) = pathsToExclude |> List.exists (path.contains StringComparison.OrdinalIgnoreCase)|> not

let applicationName = getBuildParamOrDefault "applicationName" ""
let applicationProjFile = @".\src\" @@ applicationName @@ applicationName + @".csproj"
let buildDir  = @"bin\Release" @@ applicationName
let mergedDir = @"bin\Release" @@ applicationName + ".Merged"
let release = LoadReleaseNotes "RELEASE_NOTES.md"

let projectName = "Proteus"
let projectSummary = "Elders.Proteus"
let projectDescription = "Elders.Proteus"
let projectAuthors = ["Nikolai Mynkow"; "Simeon Dimov";]

let packages = ["Proteus", projectDescription]
let nugetDir = "./bin/nuget"
let nugetDependencies = ["protobuf-net", "2.0.0.668";]
let nugetDependenciesFlat, _ = nugetDependencies |> List.unzip
let excludeNugetDependencies = excludePaths nugetDependenciesFlat

Target "Clean" (fun _ -> CleanDirs [buildDir])

Target "AssemblyInfo" (fun _ ->
    let assInfoFile = @"./src/" + applicationName + "/Properties/AssemblyInfo.cs"
    CreateCSharpAssemblyInfo assInfoFile
           [Attribute.Title "Elders.Proteus"
            Attribute.Description "Elders.Proteus"
            Attribute.Product "Elders.Proteus"
            Attribute.Version release.AssemblyVersion
            Attribute.InformationalVersion release.AssemblyVersion
            Attribute.FileVersion release.AssemblyVersion]
)

Target "Build" (fun _ ->
    Console.WriteLine applicationProjFile
    !! applicationProjFile
        |> MSBuildRelease buildDir "Build"
        |> Log "Build-Output: "
)

Target "RestorePackages" (fun _ ->
    !! "./**/packages.config"
    |> Seq.iter (RestorePackage (fun p -> { p with OutputPath = "./src/packages" }))
)

Target "CreateNuGet" (fun _ ->
    for package,description in packages do
    
        let nugetToolsDir = nugetDir @@ "lib" @@ "net45-full"
        CleanDir nugetToolsDir
        
        CreateDir mergedDir
        let mergedDll = mergedDir @@ applicationName + ".dll"
        let primary = buildDir @@ applicationName + ".dll"
        let secondary = buildDir @@ "protobuf-net.dll"
        let paramss = "/out:" + mergedDll + " " + primary + " " + secondary + " /internalize /targetplatform:v4"
        let result = ExecProcess (fun info ->  
            info.FileName <- "./tools/ILMerge/ILMerge.exe"
            info.Arguments <- paramss) System.TimeSpan.MaxValue

        match package with
        | p when p = projectName ->
            CopyDir nugetToolsDir mergedDir excludeNugetDependencies
        | _ -> ()
        
        let nuspecFile = package + ".nuspec"
        NuGet (fun p ->
            {p with
                Authors = projectAuthors
                Project = package
                Description = description
                Version = release.NugetVersion
                Summary = projectSummary
                ReleaseNotes = release.Notes |> toLines
                Dependencies = nugetDependencies
                AccessKey = getBuildParamOrDefault "nugetkey" ""
                Publish = hasBuildParam "nugetkey"
                ToolPath = "./tools/NuGet/nuget.exe"
                OutputPath = nugetDir
                WorkingDir = nugetDir }) nuspecFile
)

Target "Release" (fun _ ->

    StageAll ""
    let notes = String.concat "; " release.Notes
    Commit "" (sprintf "Bump version to %s. %s" release.NugetVersion notes)
    Branches.push ""

    Branches.tag "" release.NugetVersion
    Branches.pushTag "" "origin" release.NugetVersion
)

// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "AssemblyInfo"
    ==> "Build"
    ==> "CreateNuGet"
    ==> "Release"
 
// start build
RunParameterTargetOrDefault "target" "Build"