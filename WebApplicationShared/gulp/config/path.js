import * as nodePath from "path";

const rootFolder = nodePath.basename(nodePath.resolve());
const buildFolder = "wwwroot";
const srcFolder = `./src`;

export const path = {
  build: {
    js: `${buildFolder}/js/`,
    images: `${buildFolder}/images/`,
    css: `${buildFolder}/css/`,
    fonts: `${buildFolder}/fonts/`,
    files: `${buildFolder}/files/`,
  },
  src: {
    js: `${srcFolder}/js/app.js`,
    images: `${srcFolder}/img/**/*.{jpg,png,svg,gif,ico,webp,JPG,PNG,SVG,GIF,ICO,WEBP}`,
    svgicons: `${srcFolder}/svgicons/*.svg`,
    scss: `${srcFolder}/scss/main.scss`,
    files: `${srcFolder}/files/**/*.*`,
  },
  watch: {
    js: `${srcFolder}/js/**/*.js`,
    images: `${srcFolder}/img/**/*.{jpg,png,svg,gif,ico,webp,JPG,PNG,SVG,GIF,ICO,WEBP}`,
    scss: `${srcFolder}/scss/**/*.scss`,
    files: `${srcFolder}/files/**/*.*`,
  },
  clean: [
    `${buildFolder}/css/main.css`,
    `${buildFolder}/css/main.min.css`,
    `${buildFolder}/js/script.js`,
    `${buildFolder}/js/script.min.js`,
    `${buildFolder}/images/**/*.{jpg,png,svg,gif,ico,webp,JPG,PNG,SVG,GIF,ICO,WEBP}`,
  ],
  buildFolder: buildFolder,
  srcFolder: srcFolder,
  rootFolder: rootFolder,
};
