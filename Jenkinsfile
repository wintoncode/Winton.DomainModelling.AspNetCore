#!groovy

node {
	step([$class: 'StashNotifier'])
	try {
		stage("Clone") {
			checkout scm
		}

		stage("Restore") {
			bat "dotnet restore --source https://packages/repository/nuget-group-libs/"
		}

		stage("Build") {
			bat("\"${tool 'MSBuild15'}\" /p:Configuration=Release /p:GetVersion=True /p:WriteVersionInfoToBuildLog=True")
		}

		stage("Test") {
			def tests = [
			    "unit-tests": { test("Winton.DomainModelling.AspNetCore.Tests") }
			]
			parallel tests
		}

		stage("Publish") {
			dir("src\\Winton.DomainModelling.Abstractions\\bin") {
				bat("dotnet nuget push **\\*.nupkg --source https://packages/repository/nuget-hosted-libs/")
			}
		}

		stage("Archive") {
			archive "**\\*.nupkg"
		}

		currentBuild.result = "SUCCESS"
	}
	catch (err) {
		currentBuild.result = "FAILURE"
		throw err
	}
	finally{
		step([$class: 'StashNotifier'])
	}
}

def test(testProjectName) {
	dir("test") {
		dir("${testProjectName}") {
			bat("dotnet test --configuration Release --no-restore --no-build")
		}
	}
}