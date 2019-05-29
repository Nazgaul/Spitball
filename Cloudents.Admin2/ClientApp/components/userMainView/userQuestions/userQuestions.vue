<template>
<v-container  class="item-wrap">
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
                loading: false,
                scrollFunc: {
                    page: 0,
                    getData: this.getUserQuestionsData,
                    scrollLock: false,
                    wasCalled: false
                },
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
            handleScroll(event) {
                let offset = 6000;
                if (event.target.scrollHeight - offset < event.target.scrollTop) {
                    if (!this.scrollFunc.scrollLock) {
                        this.scrollFunc.scrollLock = true;
                        this.scrollFunc.getData(this.userId, this.scrollFunc.page)
                    }
                }
            },
            attachToScroll() {
                let containerElm = document.querySelector('.item-wrap');
                containerElm.addEventListener('scroll', this.handleScroll)
            }
        },
        created() {
            this.getUserQuestionsData();
            console.log('hello created' + this.userId);
            this.$nextTick(function () {
                this.attachToScroll();
            })
        },
        beforeDestroy() {
            let containerElm = document.querySelector('.item-wrap');
            if (!containerElm) return;
            containerElm.removeEventListener('scroll', this.handleScroll);
        }

    }
</script>

<style scoped>

</style>