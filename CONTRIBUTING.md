# Contributing

When contributing to this repository, please first discuss the change you wish to make via issue with the owners of this repository before making a change.

## Pull Request Process

We use github to host code, to track issues and feature requests, as well as accept pull requests.

Pull requests are the best way to propose changes to the codebase (we use [Github Flow](https://guides.github.com/introduction/flow/index.html)). We actively welcome your pull requests:

1. Fork the repo and create your branch from master.
2. If you've added code that should be tested, add tests.
3. If you've changed APIs, update the documentation.
4. Increase the `VersionPrefix` in [Dodo.HttpClientExtensions.csproj](src/Dodo.HttpClientExtensions/Dodo.HttpClientExtensions.csproj). The versioning scheme we use is [SemVer](https://semver.org/).
5. Ensure the test suite passes.
6. Issue that pull request.
7. Properly fill pull request body section. Best practice here is to describe your changes as a list of changes and add link to the according issue for each change.

## Report bugs and feature suggestions using Github's issues

We use GitHub issues to track public bugs and feature suggestions. Report a bug or suggest a feature by opening a new issue.

## Use a Consistent Coding Style

This project uses [EditorConfig](https://editorconfig.org/). All style settings you may find in the [.editorconfig](.editorconfig) file.

Yes, we prefer tabs over spaces in this project for *.cs files, I am so sorry about that :)

Please, use consistent code style in your contributions.

## License

By contributing, you agree that your contributions will be licensed under its [Apache License 2.0](LICENSE).

## Code of Conduct

This project has adopted the [Code of Conduct](./CODE_OF_CONDUCT.md).

## NuGet package release process for repository owners and collaborators

You can find latest releases on the [releases pane](https://github.com/dodopizza/httpclientextensions/releases) or NuGet repository. Description of the CI workflows and release process described [here](./.github/workflows/CI_AND_RELEASE.md). **Must read** if you plan to publish new release or prerelease version of the package.
