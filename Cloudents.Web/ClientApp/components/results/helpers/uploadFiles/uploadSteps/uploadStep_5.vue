<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title">Add any tag you think is relevant for this
                doc.</h3>
            <h4 class="sb-subtitle mt-2">It will help other to find you in more than one
                way</h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon class="mr-1">sbf-tag-icon</v-icon>
                    <span>Tags</span></label>
                <v-combobox class="sb-combo"
                            v-model="selectedTags"
                            height="'48px'"
                            append-icon=""
                            prepend-icon=""
                            placeholder="Type a tag name"
                            color="'#979797'"
                            multiple
                            chips
                            solo
                            :allow-overflow="false">
                    <template slot="selection" slot-scope="data" class="sb-selection">
                        <v-chip class="sb-chip-tag">
                                                   <span class="chip-button px-2">
                                                       {{!!data.item ? data.item : ''}}
                                                   </span>
                            <v-icon class="chip-close ml-3" @click="removeTag(data.item)">
                                sbf-close
                            </v-icon>
                        </v-chip>
                    </template>
                </v-combobox>
            </div>
            <div class="btn-holder">
            </div>
        </div>
    </v-card>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
     export default {
        name: "uploadStep_5",
        data() {
            return {
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
            }),
            selectedTags:{
                get () {
                    return this.getFileData.tags;
                },
                set (value) {
                    this.updateFile({'tags': value});
                }
            }
        },
        methods: {
         ...mapActions(['updateFile']),

            removeTag(item) {
                this.selectedTags.splice(this.selectedTags.indexOf(item), 1);
                this.selectedTags = [...this.selectedTags];
                this.updateFile({'tags' : this.selectedTags})
            },
        },
         created(){
            console.log('step 5 created')
         }
    }
</script>

<style >

</style>