<template>
    <v-layout class="tutoring-info-section" column align-center>
        <v-flex xs12>
            <v-card class="px-2 py-4 tutoring-info-card">
                    <v-flex>
                        <div>
                            <div class="mb-2 text-sm-center text-xs-center px-3">
                                    <span class="tutoring-info-heading">
                                         Let me know what you need and i”ll be happy to help
                                    </span>
                            </div>
                        </div>
                    </v-flex>
                    <v-flex xs12 sm12 md12 class="py-4 mb-4">
                        <contactBtn></contactBtn>
                    </v-flex>
                <div class="bottom-section px-3">
                    <div class="info-item mb-2">
                    <div>
                        <span class="tutoring-info-label">Tutoring Hours</span>
                    </div>
                    <div>
                        <span class="tutoring-info-label">10 hours</span>
                    </div>
                    </div>

                    <div class="info-item mb-2">
                        <div>
                            <span class="tutoring-info-label">Tutoring Hours</span>
                        </div>
                        <div>
                            <span class="tutoring-info-label">10 hours</span>
                        </div>
                    </div>

                    <div class="info-item ">
                        <div>
                            <span class="tutoring-info-label">Tutoring Hours</span>
                        </div>
                        <div>
                            <span class="tutoring-info-label">10 hours</span>
                        </div>
                    </div>
                </div>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import contactBtn from '../../profileHelpers/contactBtn/contactBtn.vue';
    export default {
        name: "tutorInfoBlock",
        components: {contactBtn}
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .tutoring-info-section {
        .tutoring-info-card{
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
        }
        .bottom-section{
            display: flex;
            flex-direction: column;
            width: 100%;
            justify-content: space-between;
        }
        .info-item{
            display: flex;
            justify-content: space-between;
        }
        .tutoring-info-heading {
                font-size: 14px;
                font-weight: 600;
                font-style: italic;
                color: @profileTextColor;
        }
        .tutoring-info-label{
            font-size: 14px;
            line-height: 1.46;
            color: @textColor;
        }
        .tutoring-info-value{
            font-size: 14px;
            font-weight: 600;
            line-height: 1.46;
            color: @profileTextColor;
        }
    }

</style>