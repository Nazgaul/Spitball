<template functional>
    <v-layout class="d-wrapper" column>
        <flex class="d-header pt-3">
            <v-layout row>
                <v-flex class="text-xs-right">
                    <button type="button" @click="props.closeFunction">
                        <slot name="closeAction"></slot>
                    </button>
                </v-flex>
                    <slot name="extraClose"></slot>
            </v-layout>
            <div class="title-text">
                <h1 class="mt-3">Personalize Results</h1>
                <h5 v-if="props.title" class="pt-3">{{props.title}}</h5>
            </div>
            <slot name="search" v-if="props.search">
                <v-container class="pa-0" fluid grid-list-l>
                    <v-layout row justify-center>
                        <v-flex><slot name="inputField"></slot></v-flex>
                    </v-layout>
                    <v-container fluid slot="selectedItems" class="pa-0 mb-3" v-if="props.selectedCourse">
                        <v-layout row justify-center>
                            <template v-for="course in props.selectedCourse">
                                <slot name="selectedItems" :course="course"></slot>
                            </template>
                        </v-layout>
                    </v-container>
                </v-container>
                <slot name="actionButton"></slot>
            </slot>
        </flex>
        <div class="loader" v-if="props.search&&props.isLoading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <flex class="results-container" v-else-if="props.search&&!props.isLoading">
            <v-layout fluid class="d-result" row justify-start wrap>
                <slot v-if="props.items.length">
                    <slot v-for="(item, index) in props.items" name="results" :item="item"></slot>
                </slot><div v-else>
                    <div>No Results Found</div>
                    <div v-html="props.emptyText"></div>
                </div>
            </v-layout>
            <slot name="actionContent">
            </slot>
        </flex>
    </v-layout>
</template>
