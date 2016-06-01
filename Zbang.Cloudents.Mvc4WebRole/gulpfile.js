/// <binding ProjectOpened='watch' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var config = {
    src: ['bower_components/**/*.css',
    'content/**/*.css', 
    '!content/**/*.rtl.css', 
    '!content/**/*.min.css',
    '!bower_components/**/*.rtl.css',
    '!bower_components/**/*.min.css']
}

var gulp = require('gulp');
var rtlcss = require('gulp-rtlcss');
var rename = require('gulp-rename');


gulp.task('styles', function () {
    return gulp.src(config.src, { base: "./" })
        .pipe(rtlcss()) // Convert to RTL. 
        .pipe(rename({ suffix: '.rtl' })) // Append "-rtl" to the filename. 
        .pipe(gulp.dest('.')); // Output RTL stylesheets. 
});

gulp.task('watch', function () {
    return gulp.watch(config.src, ['styles']);
});

