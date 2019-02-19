<template>
    <v-container>
        <v-layout>
            <v-flex xs12>
                <answer-item
                        :filterVal="filterValue" :answers="UserAnswers"
                ></answer-item>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import answerItem from '../helpers/answerItem.vue';

    export default {
        name: "userAnswers",
        components: {answerItem},
        data() {
            return {
                scrollFunc: {
                        page: 0,
                        getData: this.getUserAnswersData,
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
                "UserAnswers",
                "filterValue"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserAnswers",
            ]),
            nextPage() {
                this.scrollFunc.page++
            },
            getUserAnswersData() {
                let self = this;
                let id = self.userId;
                let page = self.scrollFunc.page;
                self.scrollFunc.wasCalled = true;
                self.loading = true;
                self.getUserAnswers({id, page}).then((isComplete) => {
                    self.nextPage();
                    if(!isComplete){
                        self.scrollFunc.scrollLock = false;
                    }else{
                        self.scrollFunc.scrollLock  = true;
                    }
                    self.loading = false;

                });
            },
        },
        created() {
            console.log('answers created')
            this.getUserAnswersData();
        }
    }
</script>

<style scoped>

</style>