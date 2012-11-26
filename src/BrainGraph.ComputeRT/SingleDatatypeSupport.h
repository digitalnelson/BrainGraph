#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	struct TStat;

	class TStatCalc
	{
	public:
		TStatCalc()
		{
			n1 = 0, n2 = 0;
			m1 = 0, m2 = 0;
			dv1 = 0, dv2 = 0;
		}

		void IncludeValue(int groupId, float value)
		{
			if(groupId == 0)
			{
				n1++;

				float delta = value - m1;
				m1 += delta / n1;

				if(n1 > 1)
					dv1 = dv1 + delta * (value - m1);
			}
			else
			{
				n2++;

				float delta = value - m2;
				m2 += delta / n2;

				if(n2 > 1)
					dv2 = dv2 + delta * (value - m2);
			}
		}

		TStat Calculate()
		{
			float v1 = abs(dv1) / ( n1 - 1 );
			float v2 = abs(dv2) / ( n2 - 1 );
			
			float tstat = 0;
			if(v1 < 0.00000001f && v2 < 0.00000001f)
				tstat = 0;
			else
				tstat = (m1 - m2) / sqrt( ( v1 / (float)n1 ) + ( v2 / (float)n2 ) );

			TStat stat;
			stat.V1 = v1;
			stat.V2 = v2;
			stat.M1 = m1;
			stat.M2 = m2;
			stat.Value = tstat;

			return stat;
		}
		
	private:
		int n1, n2;
		float m1, m2;
		float dv1, dv2;
	};
}}}