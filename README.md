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

## Visual studio extensions

* task runner extension (only for web frontend js&css libraries)
* node js / npm (also for frontend)
* 

## building/extending the web frontend

The web frontend is a single page application which is built with gulp/bower/...

Steps to get there:
* install nodejs/npm `cinst nodejs.install` (with chocolatey)
* npm install -g yo gulp bower
* go to Application.Web/Frontend directory in a shell and type `npm install && bower install`
 * this will get all node dependencies  and javascript/js dependencies for the frontend
* if you want to create a release type gulp - this builds/minimizes the frontend files and puts them into the dist folder
 * for debugging this is not be necessary and if there is anything in dist delete it


How to extend
* add your custom js files into Frontend/app/index.html (services/directives/etc)
* Frontend/app/index.js contains the uirouter and angular module setup 
* if you want to add a web library do not use nuget, go to command line and type `bower install --save <library name>` 
 * this will install the library with dependencies and inject the correct files into index.html 
* add a new view by creating a new folder in Frontend `/app/views/<your view>` 
 * add `<your view>.html` and `<your view>.controller.js` to the folder. 
 * add the controller.js file to Frontend/app/index.html at the bottom
 * add a state to the ui router in index.js
 * your view should work


## Issues  / Solutions


* error: `wrong version of typescript` solution `tools->extensions->install typescript 1.3 for VS2013`
* error: `web site does not load correctly` solution `delete folder Fronted/bower_components execute bower install in Frontend folder from shell`
