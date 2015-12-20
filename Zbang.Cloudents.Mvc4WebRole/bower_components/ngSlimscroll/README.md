## Angular JS slimscroll (no jQuery required)

#### ngSlimscroll
AngularJS implementation of original jQuery slimScroll

Originally developed by Piotr Rochala ([http://rocha.la](http://rocha.la)) [jQuery version](https://github.com/rochal/jQuery-slimScroll)

This version also supports horizontal scrolling, please see the [example](http://demo.jankuri.com/ngSlimscroll/) for more information about configuring the horizontal scroll.

# Example
Check out [the live demo](http://demo.jankuri.com/ngSlimscroll/)

Install
-------

#### With bower:

    $ bower install ngSlimscroll
    
#### With npm
    
    $ npm install ng-slimscroll
    
#### Example Configuration (bower)
```html
<!DOCTYPE html>
<html ng-app="app">
<body ng-controller="Ctrl as ctrl">

  <div ng-slimscroll>long content ... </div>

<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0/angular.min.js"></script>
<script type="text/javascript" src="bower_components/ngSlimscroll/src/js/ngSlimscroll.js"></script>
<script type="text/javascript">
var app = angular.module('app', ['jkuri.slimscroll']);
app.controller('Ctrl', [function() {
	var self = this;
}]);
</script>
</body>
</html>
``` 

For more information please see [http://demo.jankuri.com/ngSlimscroll/](http://demo.jankuri.com/ngSlimscroll/)



