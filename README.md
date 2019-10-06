# hrcard

This application was generated using JHipster 6.3.1 and JHipster .Net Core 0.1.0, you can find documentation and help at [https://www.jhipster.tech/documentation-archive/v6.3.1](https://www.jhipster.tech/documentation-archive/v6.3.1).

## Development

Before you can build this project, you must install and configure the following dependencies on your machine:

1. [Node.js][]: We use Node to run a development web server and build the project.
   Depending on your system, you can install Node either from source or as a pre-packaged bundle.

After installing Node, you should be able to run the following command to install development tools.
You will only need to run this command when dependencies change in [package.json](package.json).

    npm --prefix ./src/Hrcard install

We use npm scripts and [Webpack][] as our build system.

Run the following commands in two separate terminals to create a blissful development experience where your browser
auto-refreshes when files change on your hard drive.

    dotnet run --verbosity normal --project ./src/Hrcard/Hrcard.csproj
    npm --prefix ./src/Hrcard start

npm is also used to manage CSS and JavaScript dependencies used in this application. You can upgrade dependencies by
specifying a newer version in [package.json](package.json). You can also run `npm update` and `npm install` to manage dependencies.
Add the `help` flag on any command to see how you can use it. For example, `npm help update`.

The `npm --prefix ./src/Hrcard run` command will list all of the scripts available to run for this project.

### Service workers

Service workers are commented by default, to enable them please uncomment the following code.

- The service worker registering script in index.html

```html
<script>
  if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('./service-worker.js').then(function() {
      console.log('Service Worker Registered');
    });
  }
</script>
```

Note: workbox creates the respective service worker and dynamically generate the `service-worker.js`

### Managing dependencies

For example, to add [Leaflet][] library as a runtime dependency of your application, you would run following command:

    npm --prefix ./src/Hrcard install --save --save-exact leaflet

To benefit from TypeScript type definitions from [DefinitelyTyped][] repository in development, you would run following command:

    npm --prefix ./src/Hrcard install --save-dev --save-exact @types/leaflet

Then you would import the JS and CSS files specified in library's installation instructions so that [Webpack][] knows about them:
Note: there are still few other things remaining to do for Leaflet that we won't detail here.

For further instructions on how to develop with JHipster, have a look at [Using JHipster in development][].

## Building for production

To build the arifacts and optimize the hrcard application for production, run:

    cd ./src/Hrcard
    rm -rf ./src/Hrcard/wwwroot
    dotnet publish --verbosity normal -c Release -o ./app/out ./Hrcard.csproj

The `./src/Hrcard/app/out` directory will contain your application dll and its depedencies.

This will concatenate and minify the client CSS and JavaScript files. It will also modify `index.html` so it references these new files.

## Testing

To launch your application's tests, run:

    dotnet test --list-tests --verbosity normal

### Client tests

// TODO

### Code quality

// TODO

## Using Docker to simplify development (optional)

// TODO

## Build a Docker image

You can also fully dockerize your application and all the services that it depends on. To achieve this, first build a docker image of your app by running:

    docker build -f ./src/Hrcard/Dockerfile -t hrcard .

Then run:

    docker run -p 80:80 hrcard

## Continuous Integration (optional)

// TODO

[node.js]: https://nodejs.org/
[yarn]: https://yarnpkg.org/
[webpack]: https://webpack.github.io/
[angular cli]: https://cli.angular.io/
[browsersync]: http://www.browsersync.io/
[jest]: https://facebook.github.io/jest/
[jasmine]: http://jasmine.github.io/2.0/introduction.html
[protractor]: https://angular.github.io/protractor/
[leaflet]: http://leafletjs.com/
[definitelytyped]: http://definitelytyped.org/
