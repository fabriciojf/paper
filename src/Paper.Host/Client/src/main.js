import Vue from 'vue'
import Vuetify from 'vuetify'
import VueAxios from 'vue-axios'
import Notify from 'vue-notifyjs'
import VueMask from 'di-vue-mask'
import axios from 'axios'
import money from 'v-money'
import VueAuth from '@websanova/vue-auth'

import 'vuetify/dist/vuetify.css'

import App from './App'
import router from './router'
import store from './store/store'
import Paper from './paper/Paper.js'

Vue.router = router

Vue.use(VueMask)
Vue.use(Vuetify)
Vue.use(VueAxios, axios)
Vue.use(money, {precision: 4})
Vue.use(Notify, {
  timeout: 5000,
  horizontalAlign: 'left',
  verticalAlign: 'bottom'
})
Vue.use(VueAuth, {
  auth: require('@websanova/vue-auth/drivers/auth/bearer.js'),
  http: require('@websanova/vue-auth/drivers/http/axios.1.x.js'),
  router: require('@websanova/vue-auth/drivers/router/vue-router.2.x.js'),
  rolesVar: 'type'
})

Vue.config.productionTip = false
// Vue.axios.defaults.baseURL = 'http://localhost:3000'

// Adiciona o plugin do Paper
var vm = new Vue()
Vue.use(Paper, { store, router, axios, vm })

const token = localStorage.getItem('user-token')
if (token) {
  axios.defaults.headers.common['Authorization'] = token
}

/* eslint-disable no-new */
new Vue({
  el: '#app',
  store,
  router,
  template: '<App/>',
  components: { App }
})
