<template>
<v-container>
    <v-layout>
        <v-flex xs12>
            <question-item
                    :filterVal="filterValue" :questions="UserQuestions"
            ></question-item>
        </v-flex>
    </v-layout>
</v-container>
</template>

<script>
    import questionItem from '../helpers/questionIitem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userQuestions",
        components: {questionItem},
        data() {
            return {
                scrollFunc: {
                    page: 0,
                    getData: this.getUserQuestionsData,
                    scrollLock: false,
                    wasCalled: false
                },

                searchQuery: ''
            }
        },
        props: {
            userId: {

            },
        },
        computed: {
            ...mapGetters([
                "UserInfo",
                "UserQuestions",
                "filterValue"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserQuestions",
            ]),
            nextPage() {
                this.scrollFunc.page++
            },
            getUserQuestionsData() {
                let id = this.userId;
                let page = this.scrollFunc.page;
                let self = this;
                self.scrollFunc.wasCalled = true;
                self.loading = true;
                self.getUserQuestions({id, page}).then((isComplete) => {
                    self.nextPage();
                    if (!isComplete) {
                        self.scrollFunc.scrollLock = false;
                    } else {
                        self.scrollFunc.scrollLock = true;
                    }
                    self.loading = false;

                });
            },
        },
        created(){
            this.getUserQuestionsData();
        }
    }
</script>

<style scoped>

</style>