var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
//import SvgSprite from 'vue-svg-sprite';
//Vue.use(SvgSprite, {
//    url: './Images/icons.svg',
//    class: 'icon'
//});
var AppComponent = (function (_super) {
    __extends(AppComponent, _super);
    function AppComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AppComponent.prototype.data = function () {
        return {
            customers: [],
            isOpen: false,
            sectionOpen: false,
            search_form: {
                type: '',
                filter: '',
            },
            unsubscribe_form: {
                email: '',
                errors: [],
            }
        };
    };
    AppComponent = __decorate([
        Component({
            components: {
                AppAside: require('./aside/navbar.vue.html'),
                AppHeader: require('./header/header.vue.html')
            }
        })
    ], AppComponent);
    return AppComponent;
}(Vue));
export default AppComponent;
//@Component({
//    components: {
//        aside: require('/aside/navmenu.vue.html'),
//        header: require('/header/header.vue.html')
//    }
//})
//new Vue({
//    components: {
//        'aside': require('/aside/navmenu.vue.html'),
//        'header': require('/header/header.vue.html')}
//}) 
//# sourceMappingURL=sections.js.map