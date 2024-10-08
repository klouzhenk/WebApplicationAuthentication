import webpack from "webpack-stream"

export const js = () => {
  return app.gulp
    .src(app.path.src.js, { sourcemaps: true })
    .pipe(
      app.plugins.plumber(
        app.plugins.notify.onError({
          title: "JS",
          message: "Error: <%= error.message %>",
        })
      )
    )
    .pipe(webpack({
        mode: 'production',
        output: {
            filename: 'app.min.js',
            library: "myLib"
        }
    }))
    .pipe(app.gulp.dest(app.path.build.js));
};
