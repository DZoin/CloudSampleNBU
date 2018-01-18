<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="LocationMatch.Cloud" generation="1" functional="0" release="0" Id="0d63b855-336d-473c-87a4-af607ed1cb2e" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="LocationMatch.CloudGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="LocationMatch.WebSite.WebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LB:LocationMatch.WebSite.WebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="LocationMatch.Analysis.WorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/MapLocationMatch.Analysis.WorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="LocationMatch.WebSite.WebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/MapLocationMatch.WebSite.WebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:LocationMatch.WebSite.WebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapLocationMatch.Analysis.WorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.Analysis.WorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapLocationMatch.WebSite.WebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="LocationMatch.Analysis.WorkerRole" generation="1" functional="0" release="0" software="E:\Cloud2017\NETB481_Week12\AzureCourse_LocationMatch\Source_MapReduce\LocationMatch.Cloud\csx\Debug\roles\LocationMatch.Analysis.WorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;LocationMatch.Analysis.WorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;LocationMatch.Analysis.WorkerRole&quot; /&gt;&lt;r name=&quot;LocationMatch.WebSite.WebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.Analysis.WorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.Analysis.WorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.Analysis.WorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="LocationMatch.WebSite.WebRole" generation="1" functional="0" release="0" software="E:\Cloud2017\NETB481_Week12\AzureCourse_LocationMatch\Source_MapReduce\LocationMatch.Cloud\csx\Debug\roles\LocationMatch.WebSite.WebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8080" />
            </componentports>
            <settings>
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;LocationMatch.WebSite.WebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;LocationMatch.Analysis.WorkerRole&quot; /&gt;&lt;r name=&quot;LocationMatch.WebSite.WebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="LocationMatch.WebSite.WebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="LocationMatch.Analysis.WorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="LocationMatch.Analysis.WorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="LocationMatch.WebSite.WebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="LocationMatch.Analysis.WorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="LocationMatch.WebSite.WebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="b598a1b2-7a74-456a-b7a7-ae6d56f2bcdb" ref="Microsoft.RedDog.Contract\ServiceContract\LocationMatch.CloudContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="1e8b7a07-73d2-4040-bad2-e8f95afb38b8" ref="Microsoft.RedDog.Contract\Interface\LocationMatch.WebSite.WebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/LocationMatch.Cloud/LocationMatch.CloudGroup/LocationMatch.WebSite.WebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>