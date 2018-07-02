﻿import debounce from "lodash/debounce"
const plusButton = () => import("./svg/plus-button.svg");
import { typesPersonalize, searchObjects } from "./consts"
const searchItemUniversity = () => import("./searchItemUniversity.vue");
const searchItemCourse = () => import("./searchItemCourse.vue");
import { mapGetters, mapMutations, mapActions } from "vuex"
import CourseAdd from "./courseAdd.vue";
import PageLayout from "./layout.vue";

export default {
    watch: {
        type(val) {
            if (val) {
                this.currentType = val;
                let title=`${val} Select`;
                this.$ga.page({
                    page: `select${val}`,
                    title,
                    location: window.location.href
                });
            }
        },
        currentType() {
            this.isLoading = false;
            this.val="";
            this.items = [];
            this.$refs.searchText ? this.$refs.searchText.focus():"";
        },
        val: debounce(function () {
            this.items = [];
            if (this.val.length > 2)
                 this.$_search();
            else{
                this.noResults=false;
            }
        }, 250),
        isShown(val){
            if(val&&this.$refs.searchText){
                this.$refs.searchText.focus();
                this.items = [];
                this.noResults=false;
            }else if(!val&&this.$refs.searchText){
                this.$refs.searchText.inputValue="";
            }
        }
    },
    computed: {
        ...mapGetters(["myCourses", "getUniversityImage", "getUniversityName", "myCoursesId"]),
        title() {
            if (this.currentAction) return "Add Class";
            if (this.currentType === "course") return this.getUniversityName;
            return "Personalize Results";
        },
        // showCreateCourse: function () {
        //     return this.val.length > 2 && this.currentType === typesPersonalize.course && !this.isLoading;
        // },

        currentItem: function () {
            return searchObjects[this.currentType];
        },
        selectedCourse() {
            if (this.currentType === "course") {
                this.items = this.items.filter(i => !this.myCoursesId.includes(i.id));
                return this.myCourses;
            }
        },
        isMobile(){return this.$vuetify.breakpoint.xsOnly;}

    },
    data() {
        return {
            items: [],
            isLoading: false,
            isChanged: false,
            currentType: "",
            currentAction: "",
            newCourseName: "",
            val: "",
            noResults:false,
            showAdd:false
        };
    },

    components: {
        CourseAdd, PageLayout, searchItemUniversity, searchItemCourse, plusButton
    },
    props: { type: { type: String, required: true }, keep: { type: Boolean }, isFirst: { type: Boolean },isShown:{type:Boolean} },
    methods: {
        ...mapMutations({ updateUser: "UPDATE_USER" }),
        // ...mapActions(["createCourse"]),
        $_removeCourse(val) {
            if(this.val){this.items.push(this.myCourses.find(i => i.id === val));}
            this.updateUser({ myCourses: this.myCourses.filter(i => i.id !== val) });
        },
        $_closeButton() {
            if (this.isFirst && this.currentType === "course" && !this.currentAction) {
                this.currentType = "university";
            } else {

                this.currentAction ? this.currentAction = "" : this.$emit('input', false);
            }
        },
        $_clickItemCallback(keep) {
            if (this.currentItem.click) {
                this.currentItem.click.call(this, keep);
            }
        },
        $_actionDone(val) {
            this.items = [... this.items, val];
            this.currentAction = "";
        },
        $_actionsCallback(action) {
            this.currentAction = action;
        },
        $_search() {
            //if (this.val.length > 2) {
            this.isLoading = true;
            //    debounce(function (val) {
            this.$store.dispatch(this.currentItem.searchApi, { term: this.val }).then(({ data: body }) => {
                this.items = body.courses||body.universities;
                this.noResults= !this.items||!this.items.length;
                this.isLoading = false;
            });
            //    }, 500);
            //}
        },

        // $_submitAddCourse() {
        //     this.createCourse({ name: this.newCourseName });
        //     this.newCourseName = "";
        //     this.$el.querySelector('.results-container').scrollTop = 0;
        //     this.val="";
        //     this.showAdd=false;
        // },
        // $_clearAddCourse() {
        //     this.newCourseName = "";
        //     this.showAdd=false;
        // }
    }
}