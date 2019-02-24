<template>
    <v-container  class="item-wrap">
        <v-layout>
            <v-flex xs12>
                <flagged-item
                        :flaggedItems="UserFlaggedItems"
                        :filterVal="filterValue"></flagged-item>
            </v-flex>
        </v-layout>
        <v-progress-circular
                style="position: absolute; top: 300px; left: auto; right: auto;"
                :size="150"
                class="loading-spinner"
                color="#00bcd4"
                v-show="loading"
                indeterminate
        >
            <span>Loading...</span>
        </v-progress-circular>
    </v-container>

</template>



<script>
    import {mapGetters, mapActions} from 'vuex';
    import flaggedItem from '../helpers/flaggedItem.vue';

    export default {
        name: "userFlaggedItems",
        components: {flaggedItem},
        data() {
            return {
                loading: false,
                scrollFunc: {
                    page: 0,
                    getData: this.getUserFlaggedData,
                    scrollLock: false,
                    wasCalled: false
                },
            }
        },
        props: {
            userId: {},
        },
        computed: {
            ...mapGetters([
                "UserInfo",
                "UserFlaggedItems",
                "filterValue"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserFlaggedItems",
            ]),
            nextPage() {
                this.scrollFunc.page++
            },
            getUserFlaggedData() {
                let self = this;
                let id = this.userId;
                let page = this.scrollFunc.page;
                this.scrollFunc.wasCalled = true;
                this.loading = true;
                self.getUserFlaggedItems({id, page}).then((isComplete) => {
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
                let offset = 2000;
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
            this.getUserFlaggedData();
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