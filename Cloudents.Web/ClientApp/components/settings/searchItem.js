import debounce from "lodash/debounce"
const plusButton = () => import("./svg/plus-button.svg");
const closeButton = () => import("./svg/close-icon.svg");
import { searchObjects } from "./consts"
const searchItemUniversity = () => import("./searchItemUniversity.vue");
const searchItemCourse = () => import("./searchItemCourse.vue");
import { mapGetters, mapMutations, mapActions } from "vuex"
import CourseAdd from "./courseAdd.vue";
import PageLayout from "./layout.vue";
import VDialog from "vuetify/src/components/VDialog/VDialog";

export default {
    model: {
        prop: "value",
        event: "change"
    },
    watch: {
        type(val) {
            this.currentType = val;
        },
        currentType() {
            this.isLoading = true;
            this.items = [];
            this.$refs.searchText ? this.$refs.searchText.inputValue = "" : this.$_search("");
        }
    },
    computed: {
        ...mapGetters(["getUniversityName", "courseFirstTime"]),
        dialog: {
            get() {
                return this.value;
            }, set(val) {
                if (!val) this.$refs.searchText.inputValue = "";
                this.$emit("change", val);
            }
        },
        title() {
            if (this.currentAction && !this.dialog) this.dialog = true;
            if (this.currentAction) return "Add Class";
            if (this.currentType === "course") return this.getUniversityName;
            return "Personalize Results";
        },
        ...mapGetters(["myCourses", "getUniversityImage", "getUniversityName"]),
        currentItem: function () {
            return searchObjects[this.currentType];
        },
        emptyText: function () { return this.currentItem.emptyState },
        selectedCourse() {
            if (this.currentType === "course")
                return this.myCourses;
        }
    },
    data() {
        return {
            items: [],
            isLoading: true,
            isChanged: false,
            currentType: "",
            currentAction: "",
            newCourseName: "",
            showAddCourse: false,
            showCourseFirst: true,
            courseFirst: false
        };
    },

    components: {
        CourseAdd, VDialog, PageLayout, searchItemUniversity, searchItemCourse, closeButton, plusButton
    },
    props: { type: { type: String, required: true }, value: { type: Boolean }, keep: { type: Boolean }, isFirst: { type: Boolean } },
    methods: {
        ...mapMutations({ updateUser: "UPDATE_USER" }),
        ...mapActions(["createCourse"]),
        $_removeCourse(val) {
            this.updateUser({ myCourses: this.myCourses.filter(i => i.id !== val) });
        },
        $_closeButton() {
            this.currentAction ? this.currentAction = "" : this.dialog = false;
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
            this.$store.dispatch(this.currentItem.searchApi, { term: val }).then(({ body }) => {
                this.items = body;
                this.isLoading = false;
            });
        }, 500),
        $_submitAddCourse() {
            this.createCourse({ name: this.newCourseName });
            this.newCourseName = "";
            this.showAddCourse = false;
        }
    },
    created() {
        this.courseFirst = this.courseFirstTime;
        this.$nextTick(() => {
            if (this.courseFirstTime) {
                this.$store.dispatch("updateFirstTime", "courseFirstTime");
            }
        
        });
    }
}