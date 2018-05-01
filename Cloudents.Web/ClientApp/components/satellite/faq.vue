<template v-once>
    <general-page>
        <div class="faq">
            <v-expansion-panel class="elevation-0" popout>
                <v-expansion-panel-content v-for="(question,i) in questions" :key="i">
                    <v-icon slot="actions" class="actions">sbf-chevron-down</v-icon>
                    <div slot="header">
                        <div>{{question.question}}</div>
                    </div>
                    <div><div class="pa-4" v-html="question.answer"></div></div>
                </v-expansion-panel-content>
            </v-expansion-panel>
        </div>
    </general-page>
</template>

<script>
import help from "../../services/satelliteService";
import generalPage from "./general.vue";
export default {
  components: { generalPage },
  data() {
    return {
      questions: null
    };
  },
  beforeRouteEnter(to, from, next) {
    help.getFaq().then(val => {
      console.log(val);
      next(vm => vm.setData(val.data));
    });
  },
  methods: {
    setData(questions) {
      this.questions = questions;
    }
  }
};
</script>

<style src="./faq.less" lang="less"></style>