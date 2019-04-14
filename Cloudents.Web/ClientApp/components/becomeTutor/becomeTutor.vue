<template>
    <div class="become-tutor-wrap">
        <v-stepper value="2">
            <v-stepper-header>
                <v-stepper-step step="1" complete>Select campaign settings</v-stepper-step>

                <v-divider></v-divider>

                <v-stepper-step step="2">Create an ad group</v-stepper-step>

                <v-divider></v-divider>

                <v-stepper-step step="3">Create an ad</v-stepper-step>
            </v-stepper-header>
            <v-stepper-items>
                <v-stepper-content v-for="n in steps"
                                   :key="`${n}-content`"
                                   :step="`become_tutor_step_`${n}">
                </v-stepper-content>
            </v-stepper-items>
        </v-stepper>
    </div>
</template>

<script>
    export default {
        name: "becomeTutor"
    };
</script>

<style lang="less">
.become-tutor-wrap{

}
</style>