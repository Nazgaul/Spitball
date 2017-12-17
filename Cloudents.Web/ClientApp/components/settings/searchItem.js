﻿import debounce from "lodash/debounce"
const plusButton = () => import("./svg/plus-button.svg");
import { searchObjects } from "./consts"
const searchItemUniversity = () => import("./searchItemUniversity.vue");
const searchItemCourse = () => import("./searchItemCourse.vue");
import { mapGetters, mapMutations, mapActions } from "vuex"
import CourseAdd from "./courseAdd.vue";
import PageLayout from "./layout.vue";

export default {
    watch: {
        type(val) {
            if(val) {
                this.currentType = val;
            }
        },
        currentType() {
            this.isLoading = true;
            this.items = [];
            this.$refs.searchText ? this.$refs.searchText.inputValue = "" : this.$_search("");
        }
    },
    computed: {
        ...mapGetters(["courseFirstTime","myCourses", "getUniversityImage", "getUniversityName","myCoursesId"]),
        title() {
            if (this.currentAction) return "Add Class";
            if (this.currentType === "course") return this.getUniversityName;
            return "Personalize Results";
        },
        currentItem: function () {
            return searchObjects[this.currentType];
        },
        selectedCourse() {
            if (this.currentType === "course"){
                this.items=this.items.filter(i=>!this.myCoursesId.includes(i.id));    
                return this.myCourses;            
            }
        }
    },
    data() {
        return {
            items: [],
            isLoading: true,
            isChanged: false,
            currentType: "",
            currentAction: "",
            newCourseName: ""
        };
    },

    components: {
        CourseAdd, PageLayout, searchItemUniversity, searchItemCourse, plusButton
    },
    props: { type: { type: String, required: true }, keep: { type: Boolean },isFirst:{type:Boolean}},
    methods: {
        ...mapMutations({ updateUser: "UPDATE_USER" }),
        ...mapActions(["createCourse"]),
        $_removeCourse(val) {
            this.items.push(this.myCourses.find(i=>i.id===val));
            this.updateUser({ myCourses: this.myCourses.filter(i => i.id !== val) });
        },
        $_closeButton() {
            if(this.isFirst&&this.currentType==="course"&&!this.currentAction){
                this.currentType="university";
            }else {

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
        $_search: debounce(function (val) {
            this.isLoading = true;
            this.$store.dispatch(this.currentItem.searchApi, { term: val }).then(({ data:body }) => {
                this.items = body;
                this.isLoading = false;
            });
        }, 500),
        $_submitAddCourse() {
            this.createCourse({ name: this.newCourseName });
            this.newCourseName = "";
        },
        $_clearAddCourse() {
            this.newCourseName = "";
        }
    }
}