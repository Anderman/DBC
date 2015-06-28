/// <reference path="bower_components/jquery.cookie-min/jquery.cookie.js" />
/// <reference path="bower_components/jquery1.11.2/index.js" />
/// <reference path="bower_components/jquery1.11.2/index.js" />
/// <binding Clean='clean' />
/// <reference path="bower_components/blockui/jquery.blockui.js" />
/// <reference path="bower_components/angular/angular.min.js" />

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  fs = require("fs");
var sass = require('gulp-sass');
var csslint = require('gulp-csslint');
var base64 = require('gulp-css-base64');

eval("var project = " + fs.readFileSync("./project.json"));

var paths = {
    metronic: "./bower_components/assets/",
    bower: "./bower_components/",
    lib: "./" + project.webroot + "/lib/"
};

gulp.task("clean", function (cb) {
    rimraf(paths.lib, cb);
});

gulp.task("copy", ["clean","sass"], function () {
    var bowerjs = {
        "bootstrap": "bootstrap/dist/**/*.js",
        "jquery": "jQuery1.11.2/index.js",
        "angularjs": "angular/angular.min.js",
        "angularjs-datatables": "angular-datatables/dist/**/*.js",
        "jquery-datatable": "datatables/media/js/jquery.datatables.min.js",
        "jquery-migrate": "jquery-migrate/jquery-migrate.min.js",
        "jquery-blockui": "blockui/jquery.blockUI.js",
        "jquery-cookie": "jquery.cookie-min/jquery.cookie.js",
        "jquery-uniform": "jquery.uniform/jquery.uniform.min.js",
        "jquery-validation": "jquery-validation/dist/jquery.validate.min.js",
        "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"
    }

    var metronicjs = {
        "Metronic": "global/scripts/metronic.js",
        "Metronic-layout": "admin/layout/scripts/layout.js"
    }

    for (var destinationDir in bowerjs) {
        gulp.src(paths.bower + bowerjs[destinationDir])
          .pipe(gulp.dest(paths.lib + destinationDir));
    }
    for (var destinationDir in metronicjs) {
        gulp.src(paths.metronic + metronicjs[destinationDir])
          .pipe(gulp.dest(paths.lib + destinationDir));
    }
    var bowercss = {
        "bootstrap": "bootstrap/dist/**/*.{css,map,ttf,svg,woff,eot}",
        "font-awesome": "font-awesome/{css,fonts}/*.*",
        "jquery-uniform/css": "jquery.uniform/themes/default/css/uniform.default.css",
        "jquery-uniform/images": "jquery.uniform/themes/default/images/*.*",
        "jquery-datatable/css": "datatables/media/css/jquery.datatables.min.css",
        "jquery-datatable/images": "datatables/media/images/*.*",
        "simple-line-icons": "simple-line-icons/{css,fonts}/*.*"
    }
    for (var destinationDir in bowercss) {
        gulp.src(paths.bower + bowercss[destinationDir])
          .pipe(gulp.dest(paths.lib + destinationDir));
    }
    gulp.src([
        paths.metronic + "css/admin/pages/login.css",
        paths.metronic + "css/admin/layout/layout.css",
        paths.metronic + "css/admin/layout/custom.css",
        paths.metronic + "css/global/components-rounded.css",
        paths.metronic + "css/global/plugins.css"
    ])
      .pipe(gulp.dest(paths.lib + "css"));
    gulp.src([
        paths.metronic + "css/admin/layout/themes/default.css"
    ])
      .pipe(gulp.dest(paths.lib + "css/themes"));

    gulp.src([
        paths.metronic + "admin/pages/img/*.*",
        paths.metronic + "admin/layout/img/*.*",
        paths.metronic + "global/img/**/*.*"
    ])
      .pipe(gulp.dest(paths.lib + "img"));

});
gulp.task('sass', function () {
    gulp.src('./bower_components/sass/**/*.scss')
      .pipe(sass().on('error', sass.logError))
      .pipe(gulp.dest('./bower_components/assets/css'));
});
gulp.task('csslint', function () {
    gulp.src(paths.lib + '/css/plugins.css')
    .pipe(csslint())
    .pipe(csslint.reporter())
    .pipe(base64())
    .pipe(gulp.dest(paths.lib + '/base64.css'));
    //.pipe(base64.reporter())
});