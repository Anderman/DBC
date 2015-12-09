/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    debug = require('gulp-debug'),
    rename = require('gulp-rename');

var paths = {
    webroot: "./wwwroot/"
};

paths.nocatjs = paths.webroot + "jsnocat/**/*.js";
paths.nocatminJs = paths.webroot + "jsnocat/**/*.min.js";
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.foreachDest = paths.webroot + "jsnocat/foreach.min.js";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(debug())
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});
gulp.task("minnocat:js", function () {
    return gulp.src([paths.nocatjs, "!" + paths.nocatminJs], { base: "." })
        .pipe(uglify())
        .pipe(gulp.dest(paths.foreachDest));
});

gulp.task("min", ["min:js", "min:css"]);
