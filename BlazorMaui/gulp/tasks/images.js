import imagemin from "gulp-imagemin";

export const images = async function() {
  console.log(app.path.src.images);
  return ( 
    app.gulp.src(app.path.src.images)
      .pipe(
        app.plugins.plumber(
          app.plugins.notify.onError({
            title: "IMAGES",
            message: "Error: <%= error.message %>",
          })
        )
      )
      .pipe( app.plugins.newer(app.path.build.images))
      .pipe(
         imagemin({
          progressive: true,
          svgoPlugins: [
            { removeViewBox: false },
            { cleanupIDs: false }
          ],
          interlaced: true,
          optimizationLevel: 3, // 0 to 7
        })
      )
      .pipe(app.gulp.dest(app.path.build.images))
  );
};
