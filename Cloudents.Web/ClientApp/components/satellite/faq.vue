<template>
    <div>
        <v-expansion-panel>
            <v-expansion-panel-content v-for="(question,i) in questions" :key="i" hide-actions>
                
                <div slot="header">
                    <div>{{question.question}}</div>
                    <div class="header__icon">
                        <v-icon>sbf-arrow-button</v-icon>
                    </div></div>
                <div v-html="question.answer"></div>
            </v-expansion-panel-content>
        </v-expansion-panel>
    </div>
</template>

<script>
    import help from "../../services/satelliteService"
    export default {
        data() {
            return {
                questions: null
            }
        },
        beforeRouteEnter(to, from, next) {
            console.log("here");
            help.getFaq().then(val => {
                console.log(val);;
                next(vm => vm.setData(val.data))
            })
        },
        methods: {
            setData(questions) {
                this.questions = questions
            }
        }
    }
</script>