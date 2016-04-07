module.exports = function (grunt) {
    grunt.initConfig({
        rtlcss: {
            myTask: {
                // task options
                options: {
                    // generate source maps
                    map: { inline: false },
                    // rtlcss options
                    opts: {
                        clean: false
                    },
                    // rtlcss plugins
                    plugins: [],
                    // save unmodified files
                    saveUnmodified: true,
                },
                expand: true,
                cwd: 'ltr/',
                dest: 'rtl/',
                src: ['**/*.css']
            }
        }
	//	bower: {
	//		install: {
	//			options: {
	//				targetDir: "wwwroot/lib",
	//				layout: "byComponent",
	//				cleanTargetDir: false
	//			}
	//		}
	//	}
	});
    //grunt.registerTask("default", ["bower:install"]);
    
    grunt.loadNpmTasks('grunt-rtlcss');
    grunt.registerTask('rtlcss', ['rtlcss']);
};