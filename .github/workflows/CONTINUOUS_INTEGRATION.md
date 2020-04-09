# Continuous Integration

This project has adopted [GitHub Flow](https://guides.github.com/introduction/flow/index.html) for development lifecycle.

Also Continuous Integration (CI) and some routine actions are achieved using [GitHub Actions](https://github.com/features/actions).

## Workflows

There are several workflows to react on different GitHub events:

- [Continuous Integration](./ci.yml)
  - _Purpose_: Build library and run tests to ensure that changes doesn't broke anyhing.
  - _Run conditions_: runs on every `push` event to any branch except `master`.

- [Compare versions on Pull Request](./pull-request.yml)
  - _Purpose_: Check that library version in the PR source branch is greater than library version in the target branch.
  - _Run conditions_: on every `pull request` to `master` branch.

- [master](./master.yml)
  - _Purpose_: Build and run tests on `master` branch and create draft for the release.
  - _Run conditions_: runs on every `push` event to `master` branch.

- [release](./release.yml)
  - _Purpose_: Publish [NuGet](https://www.nuget.org/) pacakge on release.
  - _Run conditions_: runs on every `release published` event.

## How to publish new release

1. On every `push` event to `master` branch there is created draft for the future release (automated with `master` workflow).
2. Double check library version in the `VersionPrefix` field in [HttpClientExtensions.csproj](/src/HttpClientExtensions/HttpClientExtensions.csproj). It should be higher than latest release.
3. Double check release tag. It should be the same as library version with `v` prefix.
4. You have to check release notes in the release draft. It is good practices to describe all changes in the release and add links to the issues for each change.
5. Publish the release. `release workflow` will publish new release to NuGet and add NuGet package asset to the release.

## How to publish new pre-release

Process nearly the same as for new release, but you should create releasse draft manually and check `prerelease` checkbox.
NuGet package will be prefixed with the branch name.
