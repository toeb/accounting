# Frontend

The frontend is build with the [yeoman](http://yeoman.io/) angular project template.

## Requirements

* Node.js (http://nodejs.org/)
* npm (comes bundled with Node.js)
* git

## Setup

Execute following command:

```
$ npm install --global yo bower grunt-cli
```

Then open **Gruntfile.js** and check if the *apiEndpoint* is set correct:

```javascript
ngconstant: {
  ...
  // Environment targets
  development: {
    options: {
      dest: '<%= yeoman.app %>/scripts/config.js'
    },
    constants: {
      ENV: {
        name: 'development',
        apiEndpoint: 'http://localhost:11433' // must point to ASP.NET web service
      }
    }
  },
  ...
```

## Running the frontend

To run the frontend in development mode execute:

```
$ grunt serve
```

This starts a web server on port **8080**.

## Preparing for production

To build the frontend for production execute:

```
$ grunt build
```

This builds the project and does all kind of optimization (e.g. css minification).
You can find the build project under the **dist** directory.

## Development Notes

### Adding AngularJS components

```
yo angular:route routename // adds controller, view and configures routeProvider
yo angular:controller controllername
yo angular:service servicename
```

### Installing new javascript libraries


```
bower search angular-i18n // searches for packages named angular-i18n
bower install --save angular-i18n // installes the library
```

## Possible errors

 * **unable to find local grunt**: ```npm install grunt --save-dev```

Execute following commands when project does not start:

```
npm install
bower install
```
