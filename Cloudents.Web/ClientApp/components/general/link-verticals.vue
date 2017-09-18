<template>
    <div>
        <ul>
            <li>
                <router-link :to="{name:vertical.name}" v-for="vertical in verticals" :key="vertical.name" v-bind:class="{selected: vertical.name === selected}">
                    <button v-on:click="change(vertical)" type="button">
                        <div class="round-icon">
                            <component v-bind:is="vertical.image"></component>
                        </div>
                        {{vertical.name}}
                    </button>
                    <!--<component v-bind:is="vertical.image"></component>--><span>{{vertical.name}}</span>
                </router-link>
            </li>
          
            <!--<router-link :to="vertical.name">
                <li>
                    <component v-bind:is="vertical.image"></component><span>{{vertical.name}}</span>
                </li>
            </router-link>-->
        </ul>
        <!--<carousel :scrollPerPage="false" :perPage="7">
            <slide v-for="vertical in verticals" :key="vertical.name" v-bind:class="{selected: vertical === selected}">
                <button v-on:click="change(vertical)" type="button">
                    <div class="round-icon">
                        <component v-bind:is="vertical.image"></component>
                    </div>
                    {{vertical.name}}
                </button>
            </slide>
        </carousel>-->
    </div>
</template>


<script src="./vertical-collection.js">
</script>
<style src="./vertical-collection.less" lang="less" scoped></style>