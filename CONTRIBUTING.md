Contributing
=========

We would love for you to contribute to this custom control and help make it even better than it is today! As a contributor, here are the guidelines we would like you to follow:

* [Issues and Bugs](#issue)
* [Feature Requests](#feature)
* [Submission Guidelines](#submit)
* [Commit Message Guidelines](#commit)

## <a name="issue"></a> Found an Issue?

If you find a bug in the source code or a mistake in the documentation, you can help us by [submitting an issue](#submit) to our [GitHub Repository](https://github.com/ShaneYu/ShaneYu.HotCommander.Core). Even better, you can [submit a Pull Request](#submit-pr) with a fix.

## <a name="feature"></a> Want a Feature?

You can request a new feature by [submitting an issue](#submit) to our [GitHub Repository](https://github.com/ShaneYu/ShaneYu.HotCommander.Core). If you would like to implement a new feature, please submit an issue with a proposal for your work first, to be sure that we can use it.

## <a name="submit"></a> Submitting and Issue

Before you submit an issue, search the archive, maybe your question was already answered.

If your issue appears to be a bug, and hasn't been reported, open a new issue. Help us to maximize the effort we can spend fixing issues and adding new features, by not reporting duplicate issues. Providing the following information will increase the chances of your issue being dealt with quickly:

* Overview of the Issue - if an error is being thrown a non-minified stack trace helps
* Version - what version of the control is affected (e.g. 1.0-alpha.1)
* Motivation for or Use Case - explain what are you trying to do and why the current behavior is a bug for you
* Reproduce the Error - provide an unambiguous set of steps
* Related Issues - has a similar issue been reported before?
* Suggest a Fix - if you can't fix the bug yourself, perhaps you can point to what might be causing the problem (line of code or commit)

You can file new issues by providing the above information [here](https://github.com/ShaneYu/ShaneYu.HotCommander.Core/issues/new).

## <a name="submit-pr"></a> Submitting a Pull Request (PR)

Before you submit your Pull Request (PR) consider the following guidelines:

* Search [GitHub](https://github.com/ShaneYu/ShaneYu.HotCommander.Core/pulls) for an open or closed PR that relates to your submission. You don't want to duplicate effort.
* Make your changes in a new git branch:

     ``` shell
     git checkout -b category/my-branch-name develop
     ```

  Note: replace 'category' with either 'bugfix' or 'feature' depending on what your pull request is targeting.

* Create your changes, **including unit tests**
* Run the test project to ensure that tests are still passing.
* Commit your changes using a descriptive commit message that follows our [commit message conventions](#commit). Adherence to these conventions
  is necessary because release notes are automatically generated from these messages.

     ``` shell
     git commit -a -m "your descriptive commit message"
     ```
  Note: the optional commit `-a` command line option will automatically "add" and "rm" edited files.

* Push your branch to GitHub:

    ``` shell
    git push origin category/my-branch-name
    ```

* In GitHub, send a pull request to `develop`.
* If we suggest changes then:
  * Make the required updates.
  * Re-run the test project to ensure tests are still passing.
  * Rebase your branch and force push to your GitHub repository (this will update your Pull Request):

    ``` shell
    git rebase develop -i
    git push -f
    ```

That's it! Thank you for your contribution!

#### After your pull request is merged
After your pull request is merged, you can safely delete your branch and pull the changes from the main (upstream) repository:

* Delete the remote branch on GitHub either through the GitHub web UI or your local shell as follows:

    ``` shell
    git push origin --delete category/my-branch-name
    ```

* Check out the develop branch:

    ``` shell
    git checkout develop -f
    ```

* Delete the local branch:

    ``` shell
    git branch -D category/my-branch-name
    ```

* Update your develop with the latest upstream version:

    ``` shell
    git pull --ff upstream develop
    ```

## <a name="commit"></a> Commit Message Guidelines

We have very precise rules over how our git commit messages can be formatted.  This leads to **more readable messages** that are easy to follow when looking through the **project history**.  But also, we use the git commit messages to **generate the change/release logs**.

### Commit Message Format
Each commit message consists of a **header**, a **body** and a **footer**.  The header has a special format that includes a **type**, a **scope** and a **subject**:
```
<type>(<scope>): <subject>
<BLANK LINE>
<body>
<BLANK LINE>
<footer>
```

The **header** is mandatory and the **scope** of the header is optional.

Any line of the commit message cannot be longer 100 characters! This allows the message to be easier to read on GitHub as well as in various git tools.

Footer should contain a [closing reference to an issue](https://help.github.com/articles/closing-issues-via-commit-messages/) if any.

Samples: (even more [samples](https://github.com/ShaneYu/ShaneYu.HotCommander.Core/commits/develop))
```
docs(readme): add HideAlreadySelectedItems property details
```
```
fix(demo): need to allow reordering for full demo

The demo is meant to show the full capabilities of the control, so enablement
of the reordering controls is recommended.
```

### Revert
If the commit reverts a previous commit, it should begin with `revert: `, followed by the header of the reverted commit. In the body it should say: `This reverts commit <hash>.`, where the hash is the SHA of the commit being reverted.

### Type
Must be one of the following:

* **feat**: A new feature
* **fix**: A bug fix
* **docs**: Documentation only changes
* **style**: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
* **refactor**: A code change that neither fixes a bug nor adds a feature
* **perf**: A code change that improves performance
* **test**: Adding missing tests or correcting existing tests
* **build**: Changes that affect the build system or external dependencies (example scopes: npm)
* **ci**: Changes to our CI configuration files and scripts (example scopes: appveyor)
* **chore**: Other changes that don't modify `src` or `test` files

### Scope
The scope could be anything specifying place of the commit change. For example
`README`, `BumpVersion`, etc.

### Subject
The subject contains succinct description of the change:

* use the imperative, present tense: "change" not "changed" nor "changes"
* don't capitalize first letter
* no dot (.) at the end

### Body
Just as in the **subject**, use the imperative, present tense: "change" not "changed" nor "changes".
The body should include the motivation for the change and contrast this with previous behavior.

### Footer
The footer should contain any information about **Breaking Changes** and is also the place to reference GitHub issues that this commit **Closes**.

**Breaking Changes** should start with the word `BREAKING CHANGE:` with a space or two newlines. The rest of the commit message is then used for this.

The commit message format is based on the [Angular JS](https://github.com/angular/angular/blob/master/CONTRIBUTING.md#commit-message-format) one, a detailed explanation can be found in this [document](https://docs.google.com/document/d/1QrDFcIiPjSLDn3EL15IJygNPiHORgU1_OOAqWjiDU5Y/edit#).