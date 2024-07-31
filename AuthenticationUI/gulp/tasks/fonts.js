import fs from "fs";
import fonter from "gulp-fonter";
import ttf2woff2 from "gulp-ttf2woff2";

export const otfToTtf = () => {
  //Searching .otf font files
  return (
    app.gulp
      .src(`${app.path.srcFolder}/fonts/**/*.otf`, {})
      .pipe(
        app.plugins.plumber(
          app.plugins.notify.onError({
            title: "FONTS",
            message: "Error: <%= error.message %>",
          })
        )
      )
      // converting to .ttf
      .pipe(
        fonter({
          formats: ["ttf"],
        })
      )
      // export to src folder
      .pipe(app.gulp.dest(`${app.path.srcFolder}/fonts/`))
  );
};

export const ttfToWoff = () => {
  //Searching .ttf font files
  return (
    app.gulp
      .src(`${app.path.srcFolder}/fonts/**/*.ttf`, {})
      .pipe(
        app.plugins.plumber(
          app.plugins.notify.onError({
            title: "FONTS",
            message: "Error: <%= error.message %>",
          })
        )
      )
      // converting to .woff
      .pipe(
        fonter({
          formats: ["woff"],
        })
      )
      // export to dest folder
      .pipe(app.gulp.dest(`${app.path.build.fonts}`))
      // searching .ttf fonts files
      .pipe(app.gulp.src(`${app.path.srcFolder}/fonts/*.ttf`))
      // converting to .woff2
      .pipe(ttf2woff2())
      // export to dest folder
      .pipe(app.gulp.dest(`${app.path.build.fonts}`))
  );
};

export const fontsStyle = () => {
  // styles file with linked fonts
  let fontsFile = `${app.path.srcFolder}/scss/fonts.scss`;
  // check if font files are exist
  fs.readdir(app.path.build.fonts, function (err, fontsFiles) {
    if (fontsFiles) {
      if (!fs.existsSync(fontsFiles)) {
        fs.writeFile(fontsFile, "", cb);
        let newFileOnly;
        for (let i = 0; i < fontsFiles.length; i++) {
          let fontFileName = fontsFiles[i].split(".")[0];
          if (newFileOnly !== fontFileName) {
            let fontName = fontFileName.split("-")[0]
              ? fontFileName.split("-")[0]
              : fontFileName;
            let fontWeight = fontFileName.split("-")[1]
              ? fontFileName.split("-")[1]
              : fontFileName;
            switch (fontWeight.toLowerCase()) {
              case "thin":
                fontWeight = 100;
                break;
              case "extralight":
                fontWeight = 200;
                break;
              case "light":
                fontWeight = 300;
                break;
              case "medium":
                fontWeight = 500;
                break;
              case "semibold":
                fontWeight = 600;
                break;
              case "bold":
                fontWeight = 700;
                break;
              case "extrabold":
                fontWeight = 800;
                break;
              case "heavy":
                fontWeight = 800;
                break;
              case "black":
                fontWeight = 900;
                break;

              default:
                fontWeight = 400;
                break;
            }
            fs.appendFile(
              fontsFile,
              `@font-face {
                    font-family: ${fontName};
                    font-display: swap;
                    src: url("../fonts/${fontFileName}.woff2") format("woff2"), url("../fonts/${fontFileName}.woff") format("woff");
                    font-weight: ${fontWeight};
                    font-style: normal;
                }\r\n`,
              cb
            );
            newFileOnly = fontFileName;
          }
        }
      } else {
        console.log(
          "File scss/fonts.scss already exists. Need to delete this file first."
        );
      }
    }
  });
  return app.gulp.src(`${app.path.srcFolder}`);
  function cb() {}
};
