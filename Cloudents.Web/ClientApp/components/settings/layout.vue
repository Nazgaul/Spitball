<template functional>
    <v-layout class="d-wrapper" column>
        <v-layout class="navigation-buttons" row justify-space-between>
            <button type="button" @click="props.closeFunction">
                <slot name="closeAction"></slot>
            </button>
            <slot :name="`${props.type}ExtraClose`"></slot>
        </v-layout>
        <div class="d-header pt-3">
            <div>
                <v-layout row align-center justify-center class="title-text mt-3">
                    <img class="mr-2 elevation-1 uni-icon" v-if="props.titleImage" :src="props.titleImage" />
                    <h1 class="">{{props.title}}</h1>
                </v-layout>
            </div>
            <slot name="search" v-if="props.search">
                <v-container class="pa-0">
                    <v-layout row justify-center>
                        <v-flex><slot name="inputField"></slot></v-flex>
                    </v-layout>
                    <v-container fluid slot="selectedItems" class="selected-items px-2 pt-3 pb-0" v-if="props.selectedCourse">
                        <v-layout row wrap justify-start>
                            <template v-for="course in props.selectedCourse">
                                <slot name="selectedItems" :course="course"></slot>
                            </template>
                        </v-layout>
                        <v-divider v-if="props.selectedCourse.length" class="mt-3 results-divider"></v-divider>
                    </v-container>
                </v-container>
            </slot>
        </div>
        <div class="loader" v-if="props.search&&props.isLoading">
            <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
        </div>
        <v-flex class="results-container px-2 mt-2" v-else-if="props.search&&!props.isLoading">
            <v-layout fluid class="d-result" row justify-start wrap>
                <template v-for="item in props.items" v-if="props.items.length">
                    <slot name="results" :item="item"></slot>
                </template>
                <slot :name="`${props.type}ExtraItem`"></slot>
                <slot :name="`${props.type}EmptyState`"></slot>
            </v-layout>
            <slot name="actionContent">
            </slot>
        </v-flex>
    </v-layout>
</template>
