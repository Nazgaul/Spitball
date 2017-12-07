
<template>
    <v-dialog v-model="dialog" fullscreen content-class="dialog-choose" v-if="currentItem" class="settings" :overlay=false>
        <page-layout :type="currentType" :title="title" :search="!currentAction" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :emptyText="emptyText" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="white--text" slot="extraClose" @click="$_closeButton" v-if="currentType!=='university'">DONE</button>
            <template slot="closeAction">
                <close-button v-if="currentType==='university'"></close-button>
                <i v-else class="sbf icon sbf-arrow-button"></i>
            </template>
            <template v-if="currentType==='course'" slot="courseExtraItem">
                <div class="add-course-form ma-2 py-3 px-3">
                    <form @submit.prevent="$_submitAddCourse">
                        <div class="form-title">Still don't see your class?</div>
                        <v-text-field dark v-model="newCourseName" placeholder="Type it in here:"></v-text-field>
                        <div class="actions">
                            <v-btn :disabled="!newCourseName" @click="$_submitAddCourse">save</v-btn>
                            <v-btn :disabled="!newCourseName" @click="$_clearAddCourse">clear</v-btn>
                        </div>
                    </form>
                </div>
            </template>

            <v-text-field light solo slot="inputField" @input="$_search" class="search-b" ref="searchText" :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>

            <v-chip class="ma-2" slot="selectedItems" slot-scope="props" v-if="selectedCourse" label>
                <span class="name">{{ props.course.name }}</span>
                <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                    <close-button></close-button>
                </button>
            </v-chip>
            <v-flex class="result" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
                <component :is="'search-item-'+currentType" :item="props.item"></component>
            </v-flex>
            <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
        </page-layout>
    </v-dialog>
</template>
<script src="./searchItem.js">
</script>
<style src="./searchItem.less" lang="less"></style>
