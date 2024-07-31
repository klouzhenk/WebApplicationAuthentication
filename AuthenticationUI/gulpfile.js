import gulp from "gulp"; // main module
import { path } from "./gulp/config/path.js"; // importing path
import { plugins } from "./gulp/config/plugins.js"; // importing plugins

// importing tasks
import { reset } from "./gulp/tasks/reset.js";
import { copy } from "./gulp/tasks/copy.js";
import { scss } from "./gulp/tasks/scss.js";
import { js } from "./gulp/tasks/js.js";
import { images } from "./gulp/tasks/images.js";
import { otfToTtf, ttfToWoff, fontsStyle } from "./gulp/tasks/fonts.js";
import { svgIcons } from "./gulp/tasks/svgIcons.js";

// creating global app props
global.app = {
  path: path,
  gulp: gulp,
  plugins: plugins
};

// filse watcher
function watcher() {
  gulp.watch(path.watch.files, copy);
  gulp.watch(path.watch.scss, scss);
  gulp.watch(path.watch.js, js);
  gulp.watch(path.watch.images, images);
}

// exporting function for creation svg sprites
export { svgIcons };

// creating fonts tasks
const fonts = gulp.series(otfToTtf, ttfToWoff, fontsStyle);

// creating task list
const mainTasks = gulp.series(
  fonts,
  gulp.parallel(copy, scss, js, images)
);

// creating gulp scripts
const dev = gulp.series(reset, mainTasks, watcher);
const build = gulp.series(reset, mainTasks);

// export scripts
export { dev };
export { build };

// execute default tasks
gulp.task("default", dev);