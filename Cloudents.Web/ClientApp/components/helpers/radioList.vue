<template functional>
    <v-expansion-panel expand>
        <v-expansion-panel-content v-for="(k,index) in props.values" :key="k.modelId" hide-actions v-bind:value=true>
            <template slot="header">
                <div>{{k.title}}</div>
                <div class="header__icon">
                    <v-icon>sbf-arrow-button</v-icon>
                </div>
            </template>
            <div class="sort-filter pa-2">
                <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter">
                    <input type="checkbox" :id="s" :checked="props.checkesVals.includes(s.id?s.id.toString():s.toString())" @change="props.callback({id:k.modelId,val:(s.id?s.id:s),type:$event})"/>
                    <label :for="s">{{s.name?s.name:s}}</label>
                </div>
                <slot :name="`${k.modelId}EmptyState`" v-if="k.data.length===0"></slot>
                <slot :name="`${k.modelId}ExtraState`" v-else></slot>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>
</template>
<style src="./radioList.less" lang="less"></style>
