/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var rtlcss = require('gulp-rtlcss');
var rename = require('gulp-rename');
var del = require('del');

var config = {
    src: ['content/**/*.css', '!content/**/*.rtl.css', '!content/**/*.min.css']
}

gulp.task('clean', function () {
    return del('Content/a.txt');
});
gulp.task('styles', function () {
    return gulp.src(config.src, { base: "./" })
        //.pipe(autoprefixer(["last 2 versions", "> 1%"])) // Other post-processing. 
        //.pipe(gulp.dest('dist')) // Output LTR stylesheets. 
        .pipe(rtlcss()) // Convert to RTL. 
        .pipe(rename({ suffix: '.rtl' })) // Append "-rtl" to the filename. 
        .pipe(gulp.dest('.')); // Output RTL stylesheets. 
    // place code for your default task here
});

