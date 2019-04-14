<template>
    <div class="become-second-wrap">
        <v-layout row wrap>
            <v-flex xs12 class="text-xs-center mb-4 mt-2">
                <span class="sharing-text">Sharing Your information and background will get more response from students</span>
            </v-flex>
            <v-flex xs12 :class="{'mt-3' : $vuetify.breakpoint.xsOnly}">
                <v-textarea
                        rows="2"
                        outline
                        v-model="description"
                        name="input-about"
                        placeholder="Type a Key sentence about yourself"
                        :label="'Opening sentence'"
                ></v-textarea>
            </v-flex>
            <v-flex xsw12>
                <v-textarea
                        rows="2"
                        outline
                        v-model="bio"
                        name="input-bio"
                        placeholder="Type a short paragraph about your background and who you are"
                        :label="'Bio'"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout  class="mt-5 px-1" :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn
                    color="#4452FC"
                    class="white-text"
                    round
                    :loading="btnLoading"
                    :disabled="btnDisabled"
                    @click="submitData()">
                <span>Done</span>
            </v-btn>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        name: "secondStep",
        data() {
            return {
                description: '',
                bio: '',
                btnLoading: false
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData']),
            btnDisabled() {
                return !this.description || !this.bio;
            }
        },
        methods: {
            ...mapActions(['updateTutorInfo', 'sendBecomeTutorData']),
            submitData() {
                let self = this;
                let descriptionAbout = {bio: self.bio, description: self.description};
                let data = {...this.becomeTutorData, ...descriptionAbout};
                self.updateTutorInfo(data);
                self.btnLoading = true;
                self.sendBecomeTutorData()
                    .then((resp) => {
                        self.btnLoading = false;
                        self.$root.$emit('becomeTutorStep', 3);
                    }, (error) => {
                        console.log('erorr sending data become tutor', error);
                        self.btnLoading = false;
                    }).finally(() => {
                    self.btnLoading = false;
                });

            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-second-wrap {

        .sharing-text {
            font-size: 14px;
            color: @textColor;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: solid 1px rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>