var gulp = require("gulp"),
    sass = require("gulp-sass"),
    autoprefixer = require("gulp-autoprefixer");
var paths = {
    webroot: "./"
}

paths.sass = paths.webroot + "scss/pages/**/*.scss";

gulp.task("sass", function () {
    return gulp.src(paths.sass, { base: "." })
        .pipe(sass.sync().on("error", sass.logError))
        .pipe(autoprefixer({
            browsers: ["> 0%"],
            cascade: false
        }))
        .pipe(gulp.dest('.'));
});

gulp.task("watch", function () {
    gulp.watch([paths.webroot + "scss/**/*.scss", paths.webroot + "scss/**/_*.scss"], ["sass"]);
})

gulp.task("default", ["sass", "watch"]);