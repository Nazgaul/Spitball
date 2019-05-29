<template>
    <v-container class="item-wrap">
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
                loading: false,
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
            handleScroll(event) {
                let offset = 2000;
                if (event.target.scrollHeight - offset < event.target.scrollTop) {
                    if (!this.scrollFunc.scrollLock) {
                        this.scrollFunc.scrollLock = true;
                        this.scrollFunc.getData(this.userId, this.scrollFunc.page )
                    }
                }
            },
            attachToScroll(){
                let containerElm = document.querySelector('.item-wrap');
                containerElm.addEventListener('scroll', this.handleScroll)
            }
        },

        created() {
            this.getUserAnswersData()
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