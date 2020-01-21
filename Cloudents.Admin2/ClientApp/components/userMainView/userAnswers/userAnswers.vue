<template>
    <!-- <v-container class="item-wrap">
        <v-layout>
            <v-flex xs12>
                <answer-item
                        :filterVal="filterValue" :answers="UserAnswers"
                ></answer-item>
            </v-flex>
        </v-layout>
    </v-container> -->
    <div>
    <v-container class="pl-2 pr-0" fluid grid-list-sm>
      <v-layout row wrap>
        <v-flex xs12 v-for="(answer, index) in filteredAnswers" :key="index">
          <answer-item :answer="answer" :filterVal="filterValue"></answer-item>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
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
                    doingStuff: false
                },
            }
        },
        props: {
            userId: {
            },
            needScroll: {
            }
        },
        computed: {
            ...mapGetters(["userAnswers", "filterValue"]),
            filteredAnswers: function() {
                return this.userAnswers.filter(f => f.state === this.filterValue);
            }
        },
         watch: {
            needScroll(val, oldval) {
            if (val && val != oldval) {
                this.getUserAnswersData();
                }
            }
        },
        methods: {
            ...mapActions(["getUserAnswers"]),
            nextPage() {
                this.scrollFunc.page++
            },
            getUserAnswersData() {
                let self = this;
                let id = self.userId;
                if (this.scrollFunc.doingStuff) {
                    return;
                }
                let page = this.scrollFunc.page;
                this.scrollFunc.doingStuff = true;
                self.getUserAnswers({ id, page }).then(isComplete => {
                    self.scrollFunc.doingStuff = !isComplete;
                    self.nextPage();
                });
            },
        },  

        created() {
            this.getUserAnswersData()
        }
    }
</script>

<style scoped>

</style>