# Introduction

This is a Outlook plug-in that can convert the edk2 community patch review mail to .patch file.

# How to Build

This project can build using Visual Studio 2019.

I tried to make it build using "dotnet.exe" but failed.

# How to Install

Copy all files in bin/Release except GitPatchExtractor.pdb to C:\Users\\<user-id>\GitPatchExtractor\ directory where \<user-id> is your windows ID.

Edit Install.reg to modify "Manifest" point to the correct path of GitPatchExtractor.vsto.

Re-launch Outlook and you should be able to see "Extract Patch" in right context menu when a patch review mail is selected.

# TODO

There are two directions.

1. Convert it from C# VSTO plug-in to JS Outlook add-in so that the add-in can be used in Outlook Web Application.
2. Make it build using "dotnet.exe" and create a fancy installer

I like #1 that someone could re-write using JS:)