# Accounting

## Guidelines

* commit only specific change sets and reference an issue with e.g. #293
* create an issue for a problem before working on its solution
* close issues
* before starting to develope read all issues
  * comment on existing issues if necessary
  * creator rejects issue if necessary
* assign issue only to yourself 
* create issues (for ideas) / add to milestone


## Requirements

* SQL Server Express
* Visual Studio 2013
  * Extensions
 

## Deploying

Server is 85.214.22.163
Domain is accounting.greeno.de

deploy settings file is saved in root


* Right click Website project in Solution Explorer
* select publish
* Choose Import 
* select accounting.greeno.de.PublishSettings in project root
* enter password for webdeploy user
* validate connection
* next
* File Publish Options 
* Database Options -> no database yet
* then publish
   * if publish fails because missing dlls be sure to set the build configuriation to the same build configuration you used in webdeploy
